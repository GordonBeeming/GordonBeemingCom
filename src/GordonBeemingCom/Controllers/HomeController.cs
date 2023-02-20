using System.Text;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Controllers;

public sealed class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private readonly AppDbContext _context;
  private readonly IConfiguration _configuration;

  public HomeController(ILogger<HomeController> logger, AppDbContext context, IConfiguration configuration)
  {
    _logger = logger;
    _context = context;
    _configuration = configuration;
  }

  [HttpGet("/")]
  public async Task<IActionResult> Index()
  {
    var now = DateTime.UtcNow;
    var viewModel = new LandingPageViewModel();
    viewModel.RecentPosts = await _context.Blogs
      .OrderByDescending(o => o.PublishDate)
      .Where(o => o.PublishDate.HasValue && o.PublishDate.Value < now)
      .Take(5)
      .Select(o => new LandingPageViewModel.Post
      {
        Id = o.Id,
        Slug = o.BlogSlug,
        Title = o.BlogTitle,
        Tags = o.BlogTags
        .Select(o => new LandingPageViewModel.Tag
        {
          Name = o.Tag.TagName,
          Slug = o.Tag.TagSlug,
        })
        .OrderBy(oo => oo.Name)
        .ToList(),
      })
      .ToListAsync();
    return View("Index", viewModel);
  }

  [Route("blog/privacy-policy")]
  public IActionResult RedirectPathPrivacy() => RedirectToActionPermanent(nameof(Privacy));

  [Route("privacy-policy")]
  public IActionResult Privacy()
  {
    return View();
  }

  [Route("robots.txt"), OutputCache(Duration = OneDayInSeconds)]
  public ContentResult RobotsText()
  {
    var productionInstance = _configuration.GetValue<bool>("ProductionInstance");
    StringBuilder stringBuilder = new StringBuilder();

    stringBuilder.AppendLine("User-agent: *");
    if (productionInstance)
    {
      stringBuilder.AppendLine("Allow: /");
    }
    else
    {
      stringBuilder.AppendLine("Disallow: /");
    }
    stringBuilder.AppendLine($"Sitemap: {ProductionUrl}/sitemap.xml");

    return this.Content(stringBuilder.ToString(), "text/plain", Encoding.UTF8);
  }
}
