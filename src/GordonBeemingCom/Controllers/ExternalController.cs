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

  public ExternalController(ILogger<ExternalController> logger, AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
  {
    _logger = logger;
    _context = context;
    _configuration = configuration;
    _httpContextAccessor = httpContextAccessor;
  }

  [HttpGet("/external")]
  public IActionResult Index([FromQuery]string link)
  {
    if (link == null)
    {
      return NotFound();
    }
    // var referrer = _httpContextAccessor.HttpContext?.Request.Headers["Referer"].ToString();
    // var host = _httpContextAccessor.HttpContext?.Request.Host.ToString();
    // var scheme = _httpContextAccessor.HttpContext?.Request.Scheme.ToString();
    // if (referrer?.StartsWith($"{scheme}://{host}/") != true)
    // {
    //   return NotFound();
    // }
    _logger.LogInformation("External link clicked {link}", link.Replace(Environment.NewLine, ""));

    // later we could track this and potentially handle redirects here instead of updating content

    return Redirect(link);
  }
}
