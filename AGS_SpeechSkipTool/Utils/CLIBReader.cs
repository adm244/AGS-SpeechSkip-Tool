using System;
using System.IO;

namespace AGS_SpeechSkipTool.Utils
{
  public class CLIBReader
  {
    private static readonly Int32 EncryptionRandSeed = 9338638;
    private static readonly UInt32 MaxDataFileLen = 50;

    private BinaryReader _reader;
    private Int32 _prevValue;

    public CLIBReader(BinaryReader reader, Int32 startValue)
    {
      _reader = reader;
      _prevValue = startValue + EncryptionRandSeed;
    }

    private Int32 GetNextPseudoRand()
    {
      _prevValue = (Int32)((long)_prevValue * 214013L + 2531011L);
      return (_prevValue >> 16) & 0x7fff;
    }

    public byte[] ReadArray(int count)
    {
      byte[] buffer = _reader.ReadBytes(count);
      for (int i = 0; i < buffer.Length; ++i)
      {
        buffer[i] = (byte)(buffer[i] - GetNextPseudoRand());
      }

      return buffer;
    }

    public byte ReadInt8()
    {
      return (byte)(_reader.ReadByte() - GetNextPseudoRand());
    }

    public Int32 ReadInt32()
    {
      byte[] bytes = ReadArray(sizeof(Int32));
      return (Int32)((bytes[3] << 24) | (bytes[2] << 16) | (bytes[1] << 8) | bytes[0]);
    }

    public string ReadString()
    {
      char[] buffer = new char[MaxDataFileLen];

      int i = 0;
      for (; i < buffer.Length; ++i)
      {
        buffer[i] = (char)((byte)(_reader.ReadByte() - GetNextPseudoRand()));
        if (buffer[i] == 0) break;
      }

      return new string(buffer, 0, i);
    }
  }
}
