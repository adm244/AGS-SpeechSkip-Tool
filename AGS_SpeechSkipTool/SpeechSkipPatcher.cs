using System;
using System.IO;
using System.Text;

namespace AGS_SpeechSkipTool
{
  public class SpeechSkipPatcher
  {
    private static readonly string CLIBSignatureHead = "CLIB\x1a";
    private static readonly string CLIBSignatureTail = "CLIB\x1\x2\x3\x4SIGE";
    private static readonly string DTASignature = "Adventure Creator Game File v2";
    private static readonly string SetSpeechSkipSignature = "\x56\x8B\x74\x24\x08\x83\xFE\x04\x77\x25";
    private static readonly string SetSpeechSkipPatchedSignature = "\xC3\x8B\x74\x24\x08\x83\xFE\x04\x77\x25";
    private static readonly byte[] OpCodeReturn = new byte[] { 0xC3 };
    private static readonly int SpeechSkipOptionIndex = 7;
    private static readonly Encoding Windows1251 = Encoding.GetEncoding(1251);

    private long _speechSkipOptionOffset = -1;
    private long _setSpeechSkipOffset = -1;

    public bool Patch(string filename, SpeechSkipType speechSkipType)
    {
      bool result = PatchExecutable(filename, speechSkipType);
      if (result == false)
        return false;

      return true;
    }

    private bool PatchExecutable(string filename, SpeechSkipType speechSkipType)
    {
      if (!IsValidExecutable(filename))
        return false;

      //MakeBackupFile(filename);

      if (!WriteChanges(filename, speechSkipType))
        return false;

      return true;
    }

    private bool WriteChanges(string filename, SpeechSkipType speechSkipType)
    {
      using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Write))
      {
        using (BinaryWriter w = new BinaryWriter(fs, Windows1251))
        {
          bool result = WriteSpeechSkipType(w, _speechSkipOptionOffset, speechSkipType);
          if (result == false)
            return false;

          result = DisableSetSpeechSkipFunction(w, _setSpeechSkipOffset);
          if (result == false)
            return false;
        }
      }

      return true;
    }

    private bool IsValidExecutable(string filename)
    {
      using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
      {
        using (BinaryReader r = new BinaryReader(fs, Windows1251))
        {
          if (!IsValidAGSGame(r))
            return false;

          long dtaOffset = GetOffsetOf(r, DTASignature);
          if (dtaOffset < 0)
            return false;

          if (!IsValidDTAFile(r, dtaOffset))
            return false;

          _speechSkipOptionOffset = GetSpeechSkipOffset(r);
          if (_speechSkipOptionOffset < 0)
            return false;

          _setSpeechSkipOffset = GetOffsetOf(r, SetSpeechSkipSignature);
          if (_setSpeechSkipOffset < 0)
          {
            _setSpeechSkipOffset = GetOffsetOf(r, SetSpeechSkipPatchedSignature);
            if (_setSpeechSkipOffset < 0)
              return false;
          }
        }
      }

      return true;
    }

    private bool DisableSetSpeechSkipFunction(BinaryWriter w, long offset)
    {
      long position = w.BaseStream.Seek(offset, SeekOrigin.Begin);
      if (position != offset)
        return false;

      w.Write(OpCodeReturn);

      return true;
    }

    private bool WriteSpeechSkipType(BinaryWriter w, long offset, SpeechSkipType type)
    {
      long position = w.BaseStream.Seek(offset, SeekOrigin.Begin);
      if (position != offset)
        return false;

      w.Write((Int32)type);

      return true;
    }

    private long GetSpeechSkipOffset(BinaryReader r)
    {
      //NOTE(adm244): stream position passed signature at this point
      uint dtaVersion = r.ReadUInt32();
      if (dtaVersion > 0xFF)
        return -1;

      string engineVersion = r.ReadPrefixedString();
      string gameName = r.ReadFixedString(50);

      //NOTE(adm244): skipping padding: sizeof(Int32) - (50 % sizeof(Int32))
      r.BaseStream.Seek(2, SeekOrigin.Current);

      r.BaseStream.Seek(sizeof(Int32) * SpeechSkipOptionIndex, SeekOrigin.Current);
      return r.BaseStream.Position;
    }

    private bool IsValidDTAFile(BinaryReader r, long offset)
    {
      r.BaseStream.Seek(offset, SeekOrigin.Begin);
      
      string headSignature = r.ReadFixedString(DTASignature.Length);
      if (!DTASignature.Equals(headSignature))
        return false;

      return true;
    }

    private bool IsValidAGSGame(BinaryReader r)
    {
      r.BaseStream.Seek(-(CLIBSignatureTail.Length + sizeof(Int32)), SeekOrigin.End);

      UInt32 clibOffset = r.ReadUInt32();
      string tailSignature = r.ReadFixedString(CLIBSignatureTail.Length);
      if (!CLIBSignatureTail.Equals(tailSignature))
        return false;

      r.BaseStream.Seek(clibOffset, SeekOrigin.Begin);

      string headSignature = r.ReadFixedString(CLIBSignatureHead.Length);
      if (!CLIBSignatureHead.Equals(headSignature))
        return false;

      return true;
    }

    private long GetOffsetOf(BinaryReader r, string pattern, string mask = null)
    {
      long originalPosition = r.BaseStream.Position;

      r.BaseStream.Seek(0, SeekOrigin.Begin);
      byte[] buffer = r.ReadBytes((int)r.BaseStream.Length);

      long resultOffset = -1;
      for (int i = 0; i < (buffer.Length - pattern.Length); ++i)
      {
        bool matched = true;
        for (int j = 0; j < pattern.Length; ++j)
        {
          if ((mask != null) && (mask[j] == '?'))
            continue;

          if (pattern[j] != buffer[i + j])
          {
            matched = false;
            break;
          }
        }

        if (matched)
        {
          resultOffset = i;
          break;
        }
      }

      r.BaseStream.Seek(originalPosition, SeekOrigin.Begin);

      return resultOffset;
    }
  }
}
