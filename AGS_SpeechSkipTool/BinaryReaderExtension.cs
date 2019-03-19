using System.IO;

namespace AGS_SpeechSkipTool
{
  public static class BinaryReaderExtension
  {
    public static string ReadFixedString(this BinaryReader r, int length)
    {
      byte[] rawBytes = r.ReadBytes(length);
      char[] rawChars = new char[rawBytes.Length];

      int i = 0;
      for (; (i < rawChars.Length); ++i)
      {
        if (rawBytes[i] == 0x0)
          break;

        rawChars[i] = (char)rawBytes[i];
      }

      return new string(rawChars, 0, i);
    }

    public static string ReadPrefixedString(this BinaryReader r)
    {
      int length = r.ReadInt32();
      return ReadFixedString(r, length);
    }
  }
}
