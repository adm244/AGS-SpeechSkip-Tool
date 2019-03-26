/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AGS_SpeechSkipTool.Extension;
using AGS_SpeechSkipTool.Utils;

namespace AGS_SpeechSkipTool.Patcher
{
  public class SpeechSkipPatcher
  {
    private static readonly string GameDataV3Filename = "game28.dta";
    private static readonly string ACV3Filename = "acwin.exe";
    private static readonly string BackupFolder = "Backup";

    private static readonly string CLIBSignatureHead = "CLIB\x1a";
    private static readonly string CLIBSignatureTail = "CLIB\x1\x2\x3\x4SIGE";
    private static readonly string DTASignature = "Adventure Creator Game File v2";

    private static readonly Signature[] SetSpeechSkipSignatures = new Signature[] {
      new Signature("\x00\x8B\x79\x04\x83\xFF\x06\x77", -0x14, "?xxxxxxx"), // v3.3
      new Signature("\x56\x8B\x74\x24\x08\x83\xFE\x04\x77\x25", 0), // v3.2
      new Signature("\x56\x8B\x74\x24\x08\x85\xF6\x7C\x00\x83\xFE\x04\x7E\x00\x68\x00\x00\x00\x00\xE8\x00\x00\x00\x00\x83\xC4\x04\xA1\x00\x00\x00\x00\x85\xC0",
        0, "xxxxxxxx?xxxx?x????x????xxxx????xx"), // v3.1
    };
    private static readonly byte[] OpCodeReturn = new byte[] { 0xC3 };

    private static readonly int SpeechSkipOptionIndex = 7;
    private static readonly int DTAVersionMin = 37; // 3.1
    private static readonly int DTAVersionMax = 49; // 3.4.1 P2

    private static readonly Encoding Windows1252 = Encoding.GetEncoding(1252);

    public event Func<PatcherEventType, PatcherEventData, bool> OnPatcherEvent = null;

    public bool Patch(string filepath, SpeechSkipType speechSkipType, bool makeBackup)
    {
      IEnumerable<Asset> dtaFiles = BuildDTAAssetList(filepath);
      foreach (Asset asset in dtaFiles)
      {
        if (makeBackup && !CreateBackup(asset.Filepath))
          return false;

        if (!PatchSpeechSkipType(asset.Filepath, asset.Offset, speechSkipType))
          return false;
      }

      IEnumerable<Asset> acFiles = BuildACAssetList(filepath);
      foreach (Asset asset in acFiles)
      {
        if (makeBackup && !CreateBackup(asset.Filepath))
          return false;

        PatchSetSkipSpeechFunction(asset.Filepath);
      }

      return true;
    }

    private bool CreateBackup(string filepath)
    {
      try
      {
        string sourceFolder = Path.GetDirectoryName(filepath);
        string sourceFilename = Path.GetFileName(filepath);

        string destinationFolder = Path.Combine(sourceFolder, BackupFolder);
        string destinationPath = Path.Combine(destinationFolder, sourceFilename);

        if (!Directory.Exists(destinationFolder))
          Directory.CreateDirectory(destinationFolder);

        File.Copy(filepath, destinationPath, true);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private bool PatchSetSkipSpeechFunction(string filepath)
    {
      long patchOffset = GetOffsetOf(filepath, SetSpeechSkipSignatures);
      if (patchOffset < 0)
        return false;

      return DisableSetSpeechSkipFunction(filepath, patchOffset);
    }

    private bool DisableSetSpeechSkipFunction(string filepath, long offset)
    {
      using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Write))
      {
        using (BinaryWriter writer = new BinaryWriter(file, Windows1252))
        {
          long position = writer.BaseStream.Seek(offset, SeekOrigin.Begin);
          if (position != offset)
            return false;

          writer.Write(OpCodeReturn);
        }
      }

      return true;
    }

    private long GetOffsetOf(string filepath, Signature[] signatures)
    {
      long offset = -1;

      for (int i = 0; i < signatures.Length; ++i)
      {
        offset = GetOffsetOf(filepath, signatures[i]);
        if (offset >= 0)
          break;
      }

      return offset;
    }

    private long GetOffsetOf(string filepath, Signature signature)
    {
      using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader reader = new BinaryReader(file, Windows1252))
        {
          reader.BaseStream.Seek(0, SeekOrigin.Begin);
          byte[] buffer = reader.ReadBytes((int)reader.BaseStream.Length);

          long resultOffset = -1;
          for (int i = 0; i < (buffer.Length - signature.Pattern.Length); ++i)
          {
            bool matched = true;
            for (int j = 0; j < signature.Pattern.Length; ++j)
            {
              if ((signature.Mask != null) && (signature.Mask[j] == '?'))
                continue;

              if (signature.Pattern[j] != buffer[i + j])
              {
                matched = false;
                break;
              }
            }

            if (matched)
            {
              resultOffset = (i + signature.Offset);
              break;
            }
          }

          return resultOffset;
        }
      }
    }

    private IEnumerable<Asset> BuildACAssetList(string gamePath)
    {
      List<Asset> assets = new List<Asset>();
      assets.Add(new Asset(gamePath, 0, GetFileSize(gamePath)));

      string gameFolder = Path.GetDirectoryName(gamePath);
      foreach (string filepath in Directory.GetFiles(gameFolder))
      {
        string filename = Path.GetFileName(filepath);
        if (filename == ACV3Filename)
        {
          assets.Add(new Asset(filepath, 0, GetFileSize(filepath)));
        }
      }

      return assets;
    }

    private bool PatchSpeechSkipType(string filepath, long offset, SpeechSkipType type)
    {
      long patchOffset = GetSpeechSkipOffset(filepath, offset);
      if (patchOffset < 0)
        return false;

      return SetSpeechSkipType(filepath, patchOffset, type);
    }

    private bool SetSpeechSkipType(string filename, long offset, SpeechSkipType type)
    {
      using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Write))
      {
        using (BinaryWriter writer = new BinaryWriter(file, Windows1252))
        {
          long position = writer.BaseStream.Seek(offset, SeekOrigin.Begin);
          if (position != offset)
            return false;

          writer.Write((Int32)type);
        }
      }

      return true;
    }

    private long GetSpeechSkipOffset(string filepath, long offset)
    {
      using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader reader = new BinaryReader(file, Windows1252))
        {
          reader.BaseStream.Seek(offset, SeekOrigin.Begin);

          string headSignature = reader.ReadFixedString(DTASignature.Length);
          if (!DTASignature.Equals(headSignature))
            return -1;

          uint dtaVersion = reader.ReadUInt32();
          if (dtaVersion > 0xFF)
          {
            return -1;
          }
          else if ((dtaVersion < DTAVersionMin) || (dtaVersion > DTAVersionMax))
          {
            bool result = OnPatcherEvent(PatcherEventType.UnsupportedDTA, new PatcherEventData(dtaVersion));
            if (result == false)
              return -1;
          }

          string engineVersion = reader.ReadPrefixedString();
          string gameName = reader.ReadFixedString(50);

          //NOTE(adm244): skipping padding: sizeof(Int32) - (50 % sizeof(Int32))
          reader.BaseStream.Seek(2, SeekOrigin.Current);

          reader.BaseStream.Seek(sizeof(Int32) * SpeechSkipOptionIndex, SeekOrigin.Current);
          return reader.BaseStream.Position;
        }
      }
    }

    private IEnumerable<Asset> BuildDTAAssetList(string gamePath)
    {
      List<Asset> assets = new List<Asset>();

      long clibOffset = GetCLIBOffsetInExecutable(gamePath);
      Asset dtaInExe = ExtractDTAAsset(gamePath, clibOffset);
      if (dtaInExe != null)
        assets.Add(dtaInExe);

      string gameFolder = Path.GetDirectoryName(gamePath);
      foreach (string filepath in Directory.GetFiles(gameFolder))
      {
        string filename = Path.GetFileName(filepath);
        if (filename == GameDataV3Filename)
        {
          assets.Add(new Asset(filepath, 0, GetFileSize(filepath)));
        }
      }

      return assets;
    }

    private long GetFileSize(string filepath)
    {
      FileInfo file = new FileInfo(filepath);
      return file.Length;
    }

    private Asset ExtractDTAAsset(string filepath, long offset = 0)
    {
      IEnumerable<Asset> clibAssets = ParseCLIBAssets(filepath, offset);
      foreach (Asset asset in clibAssets)
      {
        if (asset.Filepath == GameDataV3Filename)
          return new Asset(filepath, asset.Offset, asset.Size);
      }

      return null;
    }

    private long GetCLIBOffsetInExecutable(string filepath)
    {
      using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader reader = new BinaryReader(file, Windows1252))
        {
          reader.BaseStream.Seek(-(CLIBSignatureTail.Length + sizeof(Int32)), SeekOrigin.End);

          long clibOffset = reader.ReadUInt32();
          string tailSignature = reader.ReadFixedString(CLIBSignatureTail.Length);
          if (tailSignature != CLIBSignatureTail)
            return -1;

          return clibOffset;
        }
      }
    }

    private IEnumerable<Asset> ParseCLIBAssets(string filepath, long offset = 0)
    {
      using (FileStream file = new FileStream(filepath, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader reader = new BinaryReader(file, Windows1252))
        {
          reader.BaseStream.Seek(offset, SeekOrigin.Begin);

          // verify clib signature
          string headSignature = reader.ReadFixedString(CLIBSignatureHead.Length);
          if (headSignature != CLIBSignatureHead)
            return null;

          // parse clib
          byte version = reader.ReadByte();
          byte index = reader.ReadByte();
          Int32 encryptionKey = reader.ReadInt32();

          CLIBReader clibReader = new CLIBReader(reader, encryptionKey);

          Int32 librariesCount = clibReader.ReadInt32();
          string[] libraryNames = new string[librariesCount];
          for (int i = 0; i < librariesCount; ++i)
          {
            libraryNames[i] = clibReader.ReadString();
          }

          Int32 assetsCount = clibReader.ReadInt32();

          string[] assetFilenames = new string[assetsCount];
          for (int i = 0; i < assetsCount; ++i)
          {
            assetFilenames[i] = clibReader.ReadString();
          }

          long[] assetOffsets = new long[assetsCount];
          for (int i = 0; i < assetsCount; ++i)
          {
            assetOffsets[i] = clibReader.ReadInt32() + offset;
          }

          long[] assetSizes = new long[assetsCount];
          for (int i = 0; i < assetsCount; ++i)
          {
            assetSizes[i] = clibReader.ReadInt32();
          }

          //NOTE(adm244): skip UIDs...

          Asset[] assets = new Asset[assetsCount];
          for (int i = 0; i < assets.Length; ++i)
          {
            assets[i] = new Asset(assetFilenames[i], assetOffsets[i], assetSizes[i]);
          }

          return assets;
        }
      }
    }
  }
}
