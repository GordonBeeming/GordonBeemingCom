﻿using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Azure;
using GordonBeemingCom.Areas.Blog.ViewModels;
using GordonBeemingCom.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GordonBeemingCom.Areas.Blog.Controllers;

public sealed class ViewPostController : BaseController
{
  private readonly AppDbContext _context;
  private readonly StringHelper _stringHelper;

  public ViewPostController(AppDbContext context, StringHelper stringHelper, ILogger<ViewPostController> logger) : base(logger)
  {
    _context = context;
    _stringHelper = stringHelper;
  }

  [HttpGet("blogs/byid/{id}")]
  public IActionResult ViewPostById2(Guid id) => RedirectToActionPermanent(nameof(ViewPostById), new { id });

  [HttpGet("byid/{id}")]
  public async Task<IActionResult> ViewPostById(Guid id)
  {
    var post = await _context.Blogs.Include(o => o.Category)
        .FirstOrDefaultAsync(o => o.Id == id);
    if (post == null)
    {
      return NotFound();
    }
    return RedirectToAction(nameof(Index), new
    {
      slug = post.BlogSlug,
    });
  }

  [HttpGet("blogs/{categorySlug}/{year}/{month}/{slug}")]
  public IActionResult Index(string categorySlug, int year, int month, string slug) => RedirectToActionPermanent(nameof(Index), new { year, month, slug });

  [HttpGet("blogs/{year}/{month}/{slug}")]
  public IActionResult Index(int year, int month, string slug) => RedirectToActionPermanent(nameof(Index), new { slug });

  [HttpGet("{slug}")]
  public async Task<IActionResult> Index(string slug)
  {
    var post = await _context.Blogs.Include(o => o.Category).Include(o => o.BlogTags).ThenInclude(o => o.Tag)
        .FirstOrDefaultAsync(o => o.BlogSlug == slug);
    if (post == null)
    {
      return NotFound();
    }
    var url = Url;
    var viewModel = new ViewPostViewModel();
    viewModel.Id = post.Id;
    viewModel.Title = post.BlogTitle;
    viewModel.SubTitle = _stringHelper.StripHTML(post.BlurbHtml);
    viewModel.HeroImageUrl = GetRelativeImageUrl(post.HeroImageUrl, url);
    viewModel.Category = new ViewPostViewModel.CategoryInfo
    {
      Id = post.Category.Id,
      Name = post.Category.CategoryName,
      Slug = post.Category.CategorySlug,
    };
    viewModel.Tags = post.BlogTags.Select(oo => new ViewPostViewModel.TagInfo
    {
      Name = oo.Tag.TagName,
      Slug = oo.Tag.TagSlug,
    }).OrderBy(o => o.Name).ToList();
    viewModel.ContentBlocks = await _context.BlogContentBlocks
          .Where(o => o.BlogId == viewModel.Id)
          .OrderBy(o => o.DisplayOrder)
          .Select(o => new ViewPostViewModel.ContentBlock
          {
            ContentId = o.Id,
            AddPreSpacer = o.AddPreSpacer,
            AddPostSpacer = o.AddPostSpacer == true,
            BlockType = (ContentBlockTypes)o.BlockType,
            ContextInfo = o.ContextInfo,
            DisplayOrder = o.DisplayOrder,
          })
          .ToListAsync();
    viewModel.ContentBlocks = viewModel.ContentBlocks.Select(o =>
    {
      o.ContextInfo = ReplaceHtmlUrls(o.ContextInfo, Url);
      return o;
    }).ToList();
    //viewModel.ContentBlocks = await context.BlogContentBlocks
    //    .Where(o => o.BlogId == viewModel.BlogData.BlogId)
    //    .OrderBy(o => o.DisplayOrder)
    //    .Select(o => new ContentBlock
    //    {
    //      ContentId = o.Id,
    //      AddPreSpacer = o.AddPreSpacer,
    //      AddPostSpacer = o.AddPostSpacer.Value,
    //      BlockType = (ContentBlockTypes)o.BlockType,

    //      ContextInfo = ReplaceHtmlUrls(o.ContextInfo, Url),

    //      DisplayOrder = o.DisplayOrder,
    //    })
    //    .ToListAsync();
    //viewModel.CanonicalUrl = GetAbsoluteUri(Url.Action(nameof(ViewPost), "Blog", new { year = viewModel.BlogData.PublishYear, month = viewModel.BlogData.PublishMonth, slug = viewModel.BlogData.BlogSlug, })).ToLowerInvariant();
    return View(viewModel);
  }
}
