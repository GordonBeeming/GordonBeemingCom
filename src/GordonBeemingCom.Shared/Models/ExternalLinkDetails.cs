namespace GordonBeemingCom.Shared.Models;

public sealed record ExternalLinkDetails
{
  public string UrlHash { get; set; } = default!;
  public string Url { get; set; } = default!;
  public List<KeyValuePair<string, List<string>>> Headers { get; set; } = default!;
  public bool IsSuccessStatusCode { get; set; } = default!;
  public int HttpStatusCode { get; set; } = default!;
}
