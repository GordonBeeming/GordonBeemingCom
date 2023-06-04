namespace GordonBeemingCom.Shared.Models;

public sealed class ImageContentBlockContext
{
  public string AltText { get; set; } = string.Empty;
  public string ImageUrl { get; set; } = string.Empty;
  public int Height { get; set; }
  public int Width { get; set; }
  public string Figure { get; set; } = string.Empty;

  [JsonIgnore]
  public string HeightDisplayTag => Height > 0 ? $"height='{Height}'" : string.Empty;
  [JsonIgnore]
  public string WidthDisplayTag => Width > 0 ? $"width='{Width}'" : string.Empty;
}
