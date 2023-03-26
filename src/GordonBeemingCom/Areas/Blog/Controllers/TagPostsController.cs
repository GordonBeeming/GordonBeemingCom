using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using GordonBeemingCom.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GordonBeemingCom.Areas.Blog.Controllers;

public sealed class TagPostsController : BaseController
{
  private readonly AppDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;

  public TagPostsController(AppDbContext context, ILogger<TagPostsController> logger, IHttpContextAccessor httpContextAccessor) : base(logger)
  {
    this._context = context;
    _httpContextAccessor = httpContextAccessor;
  }

  [HttpGet("blogs/{categorySlug}")]
  public IActionResult RedirectPathCategories(string categorySlug) => RedirectToActionPermanent(IndexAction, HomeControllerName, new { });

  private IOrderedQueryable<Blogs> GetBlogPosts(DateTime now) => _context.Blogs
    .Include(o => o.Category)
    .Include(o => o.BlogTags).ThenInclude(o => o.Tag)
          .Where(o => o.PublishDate.HasValue && o.PublishDate.Value < now)
          .OrderByDescending(o => o.PublishDate ?? now);

  [HttpGet("blog/tags/{slug}")]
  public IActionResult RedirectPathIndex(string slug) => RedirectToActionPermanent(IndexAction, new { slug });

  [HttpGet("tags/{slug}")]
  public async Task<IActionResult> Index(string slug)
  {
    var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    var now = DateTime.UtcNow;
    var viewModel = new TagPostsViewModel();
    var tag = await _context.Tags
      .FirstOrDefaultAsync(o => o.TagSlug == slug);
    if (tag == null)
    {
      return NotFound();
    }
    viewModel.Title = tag.TagName;
    viewModel.SubTitle = string.Empty;
    viewModel.CanonicalUrl = Url.Action(IndexAction, ViewPostControllerName, new
    {
      slug = tag.TagSlug,
    }, HttpContext.Request.Scheme)!;
    viewModel.BannerImage = HomeImageRelativePath;
    viewModel.TagPosts = await _context.BlogTags
      .Where(o => o.TagId == tag.Id)
      .Select(o => o.Blog)
      .OrderByDescending(o => o.PublishDate)
      .Where(o => isAuthenticated ? true : o.PublishDate.HasValue && o.PublishDate.Value < now)
      .Select(o => new TagPostsViewModel.Post
      {
        Id = o.Id,
        Slug = o.BlogSlug,
        Title = o.BlogTitle,
        SubTitle = o.SubTitle,
        PublishDate = o.PublishDate,
        Tags = o.BlogTags
        .Select(o => new TagPostsViewModel.Tag
        {
          Name = o.Tag.TagName,
          Slug = o.Tag.TagSlug,
        })
        .OrderBy(oo => oo.Name)
        .ToList(),
      })
      .ToListAsync();
    if (viewModel.TagPosts.Count > 0)
    {
      viewModel.TagPosts[0].AddHR = false;
    }
    return View("Index", viewModel);
  }
}
