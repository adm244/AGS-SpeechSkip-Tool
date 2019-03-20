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
