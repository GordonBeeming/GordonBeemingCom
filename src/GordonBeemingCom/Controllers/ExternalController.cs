using System.Text;
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

  public ExternalController(ILogger<ExternalController> logger, AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IExternalUrlsService externalUrlsService)
  {
    _logger = logger;
    _context = context;
    _configuration = configuration;
    _httpContextAccessor = httpContextAccessor;
    _externalUrlsService = externalUrlsService;
  }

  [HttpGet("/external")]
  public async Task<IActionResult> Index([FromQuery]string? link)
  {
    if (link is null)
    {
      return NotFound();
    }
    if (link.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
    {
      link = $"https://{link.Remove(0,7)}";
    }
    if (!(await _externalUrlsService.IsUrlRegisteredAsync(link)))
    {
      _logger.LogWarning("External link not registered {link}", link.Replace(Environment.NewLine, ""));
      return NotFound();
    }
    _logger.LogInformation("External link clicked {link}", link.Replace(Environment.NewLine, ""));

    // later we could track this and potentially handle redirects here instead of updating content

    return Redirect(link);
  }
}
