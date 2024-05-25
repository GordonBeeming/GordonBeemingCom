using System.Text;
using System.Text.RegularExpressions;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Controllers;

public sealed class ExternalController : Controller
{
  private readonly ILogger<ExternalController> _logger;
  private readonly AppDbContext _context;
  private readonly IConfiguration _configuration;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IExternalUrlsService _externalUrlsService;

  public ExternalController(ILogger<ExternalController> logger, AppDbContext context, IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor, IExternalUrlsService externalUrlsService)
  {
    _logger = logger;
    _context = context;
    _configuration = configuration;
    _httpContextAccessor = httpContextAccessor;
    _externalUrlsService = externalUrlsService;
  }

  [HttpGet("/external")]
  public async Task<IActionResult> Index([FromQuery] string? link)
  {
    if (link is null)
    {
      return NotFound();
    }

    if (link.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
    {
      link = $"https://{link.Remove(0, 7)}";
    }

    var urlFromDb = await _externalUrlsService.GetRegisteredUrlAsync(link);
    if (urlFromDb is null)
    {
      _logger.LogWarning("External link not registered {link}", link.Replace(Environment.NewLine, ""));
      if (link.StartsWith("https://github.com/GordonBeeming/GordonBeemingCom/commit/", StringComparison.OrdinalIgnoreCase) ||
          link.StartsWith("https://github.com/GordonBeeming/GordonBeemingCom/actions?query=branch", StringComparison.OrdinalIgnoreCase) ||
          link.StartsWith("https://dotnet.microsoft.com/en-us/download/dotnet/", StringComparison.OrdinalIgnoreCase))
      {
        await _externalUrlsService.AddAcceptedExternalUrlAsync(link);
        urlFromDb = await _externalUrlsService.GetRegisteredUrlAsync(link);
      }
      if (urlFromDb is null)
      {
        return NotFound();
      }
    }

    _logger.LogInformation("External link clicked {urlFromDb}", urlFromDb);

    // later we could track this and potentially handle redirects here instead of updating content
    urlFromDb = AttachMvpContributorId(urlFromDb);
    return Redirect(urlFromDb);
  }

  const string ContributionId = "DT-MVP-5000879";

  static readonly string[] _docsAndLearnChampionDomains =
  {
    "docs.microsoft.com", "learn.microsoft.com", "social.technet.microsoft.com", "azure.microsoft.com",
    "techcommunity.microsoft.com", "social.msdn.microsoft.com", "devblogs.microsoft.com", "developer.microsoft.com",
    "channel9.msdn.com", "gallery.technet.microsoft.com", "cloudblogs.microsoft.com", "technet.microsoft.com",
    "docs.azure.cn", "www.azure.cn", "msdn.microsoft.com", "blogs.msdn.microsoft.com", "blogs.technet.microsoft.com",
    "microsoft.com/handsonlabs",

    // added from https://github.com/mjisaak/skilling-champion-extension/blob/main/src/main.js
    "csc.docs.microsoft.com", "code.visualstudio.com",
  };

  /// <summary>
  /// Copied and altered from Jesse Houwing
  /// https://gist.github.com/jessehouwing/7dd997447d5267b77842f4f5bd5a5996
  /// </summary>
  /// <param name="link"></param>
  /// <returns></returns>
  static string AttachMvpContributorId(string link)
  {
    try
    {
      Uri linkUri = new Uri(link, UriKind.Absolute);

      bool match = _docsAndLearnChampionDomains.Select(domain => new Uri("https://" + domain, UriKind.Absolute)).Any(
        y => string.Equals(linkUri.Host, y.Host, StringComparison.OrdinalIgnoreCase) &&
             linkUri.AbsolutePath.StartsWith(y.AbsolutePath, StringComparison.OrdinalIgnoreCase));

      if (match)
      {
        linkUri = linkUri.Query?.Length > 0
          ? new Uri(linkUri.GetLeftPart(UriPartial.Query) + $"&WT.mc_id={ContributionId}{linkUri.Fragment}")
          : new Uri(linkUri.GetLeftPart(UriPartial.Path) + $"?WT.mc_id={ContributionId}{linkUri.Fragment}");
        link = linkUri.AbsoluteUri;
      }

      string pattern = @"^(https?://\S*?)/([a-z]{2}-[a-z]{2})(/.*)?$";
      string replacement = "$1$3";
      link = Regex.Replace(link, pattern, replacement);
    }
    catch (UriFormatException) { }

    return link;
  }
}
