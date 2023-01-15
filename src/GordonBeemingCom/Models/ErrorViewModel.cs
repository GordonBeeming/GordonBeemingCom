namespace GordonBeemingCom.Models;

public sealed class ErrorViewModel
{
#pragma warning disable CS8618
  public string? RequestId { get; set; }

  public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
