using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using GordonBeemingCom.Areas.Blog.Data;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static GordonBeemingCom.Areas.Blog.ViewModels.LandingPageViewModel;

namespace GordonBeemingCom.Areas.Blog.Controllers;

public sealed class RssFeedsController : BaseController
{
  private readonly AppDbContext _context;

  public RssFeedsController(AppDbContext context, ILogger<RssFeedsController> logger) : base(logger)
  {
    this._context = context;
  }

  [HttpGet("rss/{categorySlug}")]
  public IActionResult Rss(string categorySlug) => RedirectToActionPermanent(nameof(Rss), new { });

  [HttpGet("rss/atom")]
  public Task<IActionResult> RssAtom()
  {
    return GetRssFeed(false, true);
  }
  [HttpGet("rss")]
  public Task<IActionResult> Rss()
  {
    return GetRssFeed(false, false);
  }

  [HttpGet("rss/{categorySlug}/latest/{latestN}")]
  public IActionResult Rss(string categorySlug, int latestN) => RedirectToActionPermanent(nameof(Rss), new { latestN });

  [HttpGet("rss/latest/{latestN}/atom")]
  public Task<IActionResult> RssAtom(int latestN)
  {
    return GetRssFeed(false, true, latestN);
  }
  [HttpGet("rss/latest/{latestN}")]
  public Task<IActionResult> Rss(int latestN)
  {
    return GetRssFeed(false, false, latestN);
  }

  [HttpGet("rss-full/{categorySlug}")]
  public IActionResult RssFull(string categorySlug) => RedirectToActionPermanent(nameof(RssFull), new { });

  [HttpGet("rss-full/atom")]
  public Task<IActionResult> RssFullAtom()
  {
    return GetRssFeed(true, true);
  }
  [HttpGet("rss-full")]
  public Task<IActionResult> RssFull()
  {
    return GetRssFeed(true, false);
  }

  [HttpGet("rss-full/{categorySlug}/latest/{latestN}")]
  public IActionResult RssFull(string categorySlug, int latestN) => RedirectToActionPermanent(nameof(RssFull), new { latestN });

  [HttpGet("rss-full/latest/{latestN}")]
  public async Task<IActionResult> RssFullAtom(int latestN)
  {
    return await GetRssFeed(true, true, latestN);
  }
  [HttpGet("rss-full/latest/{latestN}/atom")]
  public async Task<IActionResult> RssFull(int latestN)
  {
    return await GetRssFeed(true, false, latestN);
  }

  private async Task<IActionResult> GetRssFeed(bool full, bool atomFeed, int latestN = int.MaxValue)
  {
    var site = $"{ProductionUrl}/{BlogArea}/";
    var now = DateTime.UtcNow;
    var data = await _context.Blogs
    .Include(o => o.Category)
    .Include(o => o.BlogTags).ThenInclude(o => o.Tag)
          .Where(o => o.PublishDate.HasValue && o.PublishDate.Value < now)
          .OrderByDescending(o => o.PublishDate ?? now)
      .Take(latestN)
      .ToListAsync();
    var feed = new SyndicationFeed("Gordon Beeming", BlogSubHeading, new Uri(site), $"{site}", DateTime.UtcNow);
    XNamespace yahooMediaNamespace = "http://search.yahoo.com/mrss/";
    XNamespace atomNamespace = "http://www.w3.org/2005/Atom";
    XNamespace dcNamespace = "http://purl.org/dc/elements/1.1/";
    feed.AttributeExtensions.Add(new XmlQualifiedName("media", "http://www.w3.org/2000/xmlns/"), yahooMediaNamespace.NamespaceName);
    feed.AttributeExtensions.Add(new XmlQualifiedName("atom", "http://www.w3.org/2000/xmlns/"), atomNamespace.NamespaceName);
    feed.AttributeExtensions.Add(new XmlQualifiedName("dc", "http://www.w3.org/2000/xmlns/"), dcNamespace.NamespaceName);
    feed.ElementExtensions.Add(new SyndicationElementExtension(new XElement(atomNamespace + "link", new XAttribute("href", Url.ActionContext.HttpContext.Request.GetDisplayUrl()), new XAttribute("rel", "self"), new XAttribute("type", "application/rss+xml"))));
    feed.ElementExtensions.Add(new SyndicationElementExtension(new XElement("image", new XElement("title", "Gordon Beeming"), new XElement("url", ProfileImageUrl), new XElement("link", ProductionUrl), new XElement("width", "80"), new XElement("height", "80"), new XElement("description", BlogSubHeading))));
    feed.Copyright = new TextSyndicationContent($"2013 - {DateTime.UtcNow.Year} Gordon Beeming");
    feed.TimeToLive = new TimeSpan(1, 0, 0);
    var items = new List<SyndicationItem>();
    foreach (var post in data)
    {
      var postUrl = Url.Action(IndexAction, ViewPostControllerName, new
      {
        slug = post.BlogSlug,
      }, HttpContext.Request.Scheme);
      var permUrl = Url.Action("ViewPostById", ViewPostControllerName, new
      {
        id = post.Id,
      }, HttpContext.Request.Scheme);
      var title = post.BlogTitle;
      string description = await GetRssHtmlContent(post, full, Url);

      var rssItem = new SyndicationItem(title, description, new Uri(postUrl!), permUrl, post.PublishDate ?? DateTime.MaxValue);
      if (post.PublishDate.HasValue)
      {
        rssItem.PublishDate = post.PublishDate.Value;
      }
      rssItem.AddPermalink(new Uri(permUrl!));
      if (!string.IsNullOrEmpty(post.HeroImageUrl))
      {
        rssItem.ElementExtensions.Add(new SyndicationElementExtension(new XElement(yahooMediaNamespace + "content", new XAttribute("url", GetAbsoluteImageUrl(post.HeroImageUrl, Url)!))));
      }
      rssItem.ElementExtensions.Add(new SyndicationElementExtension(new XElement(dcNamespace + "creator", "Gordon Beeming")));
      items.Add(rssItem);
    }
    feed.Items = items;
    var settings = new XmlWriterSettings
    {
      Encoding = Encoding.UTF8,
      NewLineHandling = NewLineHandling.Entitize,
      NewLineOnAttributes = true,
      Indent = true,
    };
    using (var stream = new MemoryStream())
    {
      using (var xmlWriter = XmlWriter.Create(stream, settings))
      {
        if (atomFeed)
        {
          feed.SaveAsAtom10(xmlWriter);
        }
        else
        {
          var rssFormatter = new Rss20FeedFormatter(feed, false);
          rssFormatter.WriteTo(xmlWriter);
          xmlWriter.Flush();
        }
      }
      return File(stream.ToArray(), "text/xml; charset=utf-8");
    }
  }

  private async Task<string> GetRssHtmlContent(Blogs post, bool full, IUrlHelper url)
  {
    if (!full)
    {
      return post.BlurbHtml;
    }
    var contentBlocks = await _context
      .BlogContentBlocks.Where(o => o.BlogId == post.Id)
      .OrderBy(o => o.DisplayOrder)
      .ToListAsync();
    var description = string.Empty;
    foreach (var contentBlock in contentBlocks)
    {
      if (contentBlock.AddPreSpacer)
      {
        description += $@"<div>&nbsp;</div>";
      }
      if ((ContentBlockTypes)contentBlock.BlockType == ContentBlockTypes.Html)
      {
        var htmlContent = JsonConvert.DeserializeObject<HtmlContentBlockContext>(contentBlock.ContextInfo);
        description += $@"
<div class='text-justify'>
  {htmlContent!.Html}
</div>
";
      }
      else if ((ContentBlockTypes)contentBlock.BlockType == ContentBlockTypes.Image)
      {
        var imageContent = JsonConvert.DeserializeObject<ImageContentBlockContext>(contentBlock.ContextInfo);
        description += $@"
<img src='{GetRelativeImageUrl(imageContent!.ImageUrl, url)}' alt='{imageContent.AltText}' title='{imageContent.AltText}' {imageContent.HeightDisplayTag} {imageContent.WidthDisplayTag} />
";
      }
      else if ((ContentBlockTypes)contentBlock.BlockType == ContentBlockTypes.Code)
      {
        var codeContent = JsonConvert.DeserializeObject<CodeContentBlockContext>(contentBlock.ContextInfo);
        description += $@"
<pre data-lang='{codeContent!.Language}'>
{codeContent.Code}
</pre>
";
      }
      if (contentBlock.AddPostSpacer == true)
      {
        description += $"<div>&nbsp;</div>";
      }
    }
    return description;
  }
}
