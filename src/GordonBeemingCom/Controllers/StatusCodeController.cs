using System.Diagnostics;
using GordonBeemingCom.Data;
using GordonBeemingCom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Controllers;

[Route("status-code")]
public class StatusCodeController : Controller
{
  private readonly AppDbContext _context;
  private readonly string[] _pathsToIgnore = new string[] { "/blog/content/", "/blog/bundles/" };

  public StatusCodeController(AppDbContext context)
  {
    this._context = context;
  }

  public IActionResult Index(int? statusCode = null)
  {
    if (statusCode.HasValue)
    {
      if (statusCode.Value == 404 || statusCode.Value == 500)
      {
        if (statusCode.Value == 404)
        {
          return Http404();
        }
        else
        {
          return Http500();
        }
      }
    }
    return View("Http500");
  }

  [ResponseCache(Duration = 30)]
  [OutputCache(Duration = 30)]
  [HttpGet("404")]
  public IActionResult Http404()
  {
    var rawUrl = Request.GetRawUrl();
    if (!_pathsToIgnore.Any(o => rawUrl.StartsWith(o, StringComparison.InvariantCultureIgnoreCase)))
    {
      var redirectTo = _context.BlogsRedirectUrl
        .Include(o => o.Blogs)
        .ThenInclude(o => o.Category)
        .FirstOrDefault(o => o.RawUrl == rawUrl);
      if (redirectTo != null)
      {
        var postUrl = Url.Action(IndexAction, ViewPostControllerName, new
        {
          area = BlogArea,
          slug = redirectTo.Blogs.BlogSlug,
        }, HttpContext.Request.Scheme);
        return RedirectPermanent(postUrl!);
      }
    }
    this.HttpContext.Response.StatusCode = 404;
    return View("Http404");
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  [OutputCache(Duration = 0, NoStore = true)]
  [HttpGet("500")]
  public IActionResult Http500()
  {
    this.HttpContext.Response.StatusCode = 500;
    return View("Http500");
  }
}
