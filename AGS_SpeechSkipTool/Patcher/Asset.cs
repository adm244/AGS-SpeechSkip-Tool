
namespace AGS_SpeechSkipTool.Patcher
{
  public class Asset
  {
    public string Filepath;
    public long Offset;
    public long Size;

    public Asset(string filepath, long offset, long size)
    {
      Filepath = filepath;
      Offset = offset;
      Size = size;
    }
  }
}
