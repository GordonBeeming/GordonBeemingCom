using Microsoft.AspNetCore.Http.Features;

namespace GordonBeemingCom.Extensions;

public static class HttpRequestExtensions
{
  public static string GetRawUrl(this HttpRequest request)
  {
    var httpContext = request.HttpContext;

    var requestFeature = httpContext.Features.GetRequiredFeature<IHttpRequestFeature>();

    return requestFeature.RawTarget;
  }
}
