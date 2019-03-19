
namespace AGS_SpeechSkipTool.Patcher
{
  public struct Signature
  {
    public string Pattern;
    public string Mask;
    public int Offset;

    public Signature(string pattern, int offset, string mask = null)
    {
      Pattern = pattern;
      Offset = offset;
      Mask = mask;
    }
  }
}
