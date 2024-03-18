using Microsoft.Extensions.Options;

namespace GordonBeemingCom.Areas.Empty_Guid.Controllers;

public sealed class HomeController : BaseController
{
  private readonly AppDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly SiteConfig _siteConfig;

  public HomeController(ILogger<HomeController> logger, AppDbContext context, IOptions<SiteConfig> siteConfig, IHttpContextAccessor httpContextAccessor) : base(logger)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
    _siteConfig = siteConfig.Value;
  }

  public IActionResult Index()
  {
    return View();
  }
}
