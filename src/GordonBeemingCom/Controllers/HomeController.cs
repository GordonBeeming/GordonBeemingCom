using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Controllers;

public sealed class HomeController : Controller
{
  private readonly ILogger<HomeController> _logger;
  private readonly AppDbContext _context;

  public HomeController(ILogger<HomeController> logger, AppDbContext context)
  {
    _logger = logger;
    _context = context;
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
  public IActionResult Privacy2() => RedirectToActionPermanent(nameof(Privacy));

  [Route("privacy-policy")]
  public IActionResult Privacy()
  {
    return View();
  }
}
