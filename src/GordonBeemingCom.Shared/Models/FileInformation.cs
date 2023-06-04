namespace GordonBeemingCom.Shared.Models;

public sealed class FileInformation
{
#pragma warning disable CS8618
  public string UniqueIdentifier { get; set; }
  public string RawUrl { get; set; }
  public string Container { get; set; }
  public string BlobName { get; set; }
  public string FileName { get; set; }
  public byte[] Binary { get; set; }
  public string ContentType { get; set; }
}
