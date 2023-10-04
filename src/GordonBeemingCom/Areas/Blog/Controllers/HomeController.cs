using System.Linq;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GordonBeemingCom.Areas.Blog.Controllers;

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

  [Route("about-me")]
  public IActionResult AboutMe()
  {
    return RedirectPermanent("/");
  }

  [HttpGet("blogs")]
  public IActionResult RedirectPathIndex() => RedirectToActionPermanent(nameof(Index));

  public Task<IActionResult> Index() => LoadPostSummaries(_siteConfig.RecentPostsCount);


  [HttpGet("all")]
  public Task<IActionResult> AllPosts() => LoadPostSummaries(int.MaxValue - 1);

  private async Task<IActionResult> LoadPostSummaries(int postCount)
  {
    var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    var now = DateTime.UtcNow;
    var viewModel = new RecentPostsViewModel();
    viewModel.RecentPosts = await _context.Blogs
      .OrderByDescending(o => o.PublishDate)
      .Where(o => isAuthenticated ? true : (o.PublishDate.HasValue && o.PublishDate.Value < now))
      .Take(postCount + 1)
      .Select(o => new RecentPostsViewModel.Post
      {
        Id = o.Id,
        Slug = o.BlogSlug,
        Title = o.BlogTitle,
        SubTitle = o.SubTitle,
        PublishDate = o.PublishDate,
        Tags = o.BlogTags
        .Select(o => new RecentPostsViewModel.Tag
        {
          Name = o.Tag.TagName,
          Slug = o.Tag.TagSlug,
        })
        .OrderBy(oo => oo.Name)
        .ToList(),
      })
      .ToListAsync();
    if (viewModel.RecentPosts.Count > 0)
    {
      viewModel.RecentPosts[0].AddHR = false;
    }
    if (viewModel.RecentPosts.Count > postCount)
    {
      viewModel.NextPostId = viewModel.RecentPosts[^1].Id;
      viewModel.RecentPosts.Remove(viewModel.RecentPosts[^1]);
    }
    return View("Index", viewModel);
  }
}
