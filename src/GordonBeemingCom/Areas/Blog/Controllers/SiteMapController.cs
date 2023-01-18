using System.Linq;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GordonBeemingCom.Areas.Blog.Controllers;

public sealed class SiteMapController : BaseController
{
  private readonly AppDbContext _context;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly SiteConfig _siteConfig;

  public SiteMapController(ILogger<SiteMapController> logger, AppDbContext context, IOptions<SiteConfig> siteConfig, IHttpContextAccessor httpContextAccessor) : base(logger)
  {
    _context = context;
    _httpContextAccessor = httpContextAccessor;
    _siteConfig = siteConfig.Value;
  }

  [HttpGet("sitemap.xml")]
  public async Task<IActionResult> Index()
  {
    string sitemapXmlUrls = string.Empty;
    var now = DateTime.UtcNow;
    var data = _context.Blogs.Include(o => o.Category).Include(o => o.BlogTags).ThenInclude(o => o.Tag)
      .Where(o => o.PublishDate.HasValue && o.PublishDate.Value < now)
      .OrderByDescending(o => o.PublishDate);
    var posts = await data.Select(o => new {o.BlogSlug, o.PublishDate}).ToListAsync();
    foreach (var post in posts)
    {
      var postUrl = Url.Action(IndexAction, ViewPostControllerName, new
      {
        slug = post.BlogSlug,
      }, HttpContext.Request.Scheme);
      sitemapXmlUrls += $@"<url>
    <loc>{postUrl}</loc>
    <lastmod>{post.PublishDate!.Value.ToString("yyyy-MM-dd")}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.7</priority>
</url>
";
    }
    return Content($@"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9""> 
<url>
    <loc>{ProductionBlogUrl}</loc>
    <lastmod>{posts.FirstOrDefault()?.PublishDate!.Value.ToString("yyyy-MM-dd")}</lastmod>
    <changefreq>daily</changefreq>
    <priority>0.6</priority>
</url>
{sitemapXmlUrls}
<url>
    <loc>{ProductionBlogUrl}abount-me</loc>
    <lastmod>{new FileInfo(typeof(SiteMapController).Assembly.Location).LastWriteTime.ToString("yyyy-MM-dd")}</lastmod>
    <changefreq>weekly</changefreq>
    <priority>0.3</priority>
</url>
</urlset>", "text/xml");
  }
}
