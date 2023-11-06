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

  public ExternalController(ILogger<ExternalController> logger, AppDbContext context, IConfiguration configuration)
  {
    _logger = logger;
    _context = context;
    _configuration = configuration;
  }

  [HttpGet("/external")]
  public IActionResult Index([FromQuery]string link)
  {
    _logger.LogInformation("External link clicked {link}", link.Replace(Environment.NewLine, ""));

    // later we could track this and potentially handle redirects here instead of updating content

    return Redirect(link);
  }
}
