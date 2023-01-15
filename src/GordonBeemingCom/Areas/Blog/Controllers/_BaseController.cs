using Microsoft.AspNetCore.Mvc;

namespace GordonBeemingCom.Areas.Blog.Controllers;

[Area(BlogArea)]
[Route(BlogArea)]
public abstract class BaseController : Controller
{
  private readonly ILogger _logger;

  public BaseController(ILogger logger)
  {
    _logger = logger;
  }

  internal string? GetRelativeImageUrl(string imageUrl, IUrlHelper url)
  {
    if (imageUrl == null)
    {
      return null;
    }
    return url.Content(imageUrl.Replace($"{ProductionBlogUrl}/", $"~/{BlogArea}/"));
  }

  internal string? GetAbsoluteImageUrl(string imageUrl, IUrlHelper url)
  {
    if (imageUrl == null)
    {
      return null;
    }
    imageUrl = imageUrl.Replace($"~/{BlogArea}/", $"{ProductionBlogUrl}/");
    if (imageUrl.StartsWith($"/{BlogArea}/"))
    {
      imageUrl = $"{ProductionBlogUrl}{imageUrl}";
    }
    return imageUrl.Replace("//","/");
  }

  internal string ReplaceHtmlUrls(string html, IUrlHelper url)
  {
    return html
          .Replace($"{ProductionBlogUrl}/", url.Content($"~/{BlogArea}/"))
          .Replace("[~/]", url.Content($"~/{BlogArea}/"));
  }
}
