
namespace AGS_SpeechSkipTool.Patcher
{
  public class Asset
  {
    public string Filename;
    public long Offset;
    public long Size;

    public Asset(string filepath, long offset, long size)
    {
      Filename = filepath;
      Offset = offset;
      Size = size;
    }
  }
}
