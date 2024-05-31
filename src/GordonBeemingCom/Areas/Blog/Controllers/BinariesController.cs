using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Net.Http.Headers;

namespace GordonBeemingCom.Areas.Blog.Controllers;

public sealed class BinariesController : BaseController
{
  private readonly IFileService _fileService;

  public BinariesController(IFileService fileService, ILogger<BinariesController> logger): base(logger)
  {
    _fileService = fileService;
  }

  [HttpGet("/" + BlogArea + "/" + IFileService.DownloadFile + "/{container}/{*blobName}")]
  [OutputCache(Duration = OneYearInSeconds)]
  [ResponseCache(Duration = OneYearInSeconds)]
  public async Task<IActionResult> DownloadFile(string container, string blobName)
  {
    var file = await _fileService.ReadFile(container, blobName);
    if (file == null)
    {
      //return NotFound();
      return RedirectPermanent("https://gordonbeeming.com/404-file.png");
    }
    HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=31536000";
    return File(file.Binary, file.ContentType, file.FileName);
  }

  [HttpGet("/" + BlogArea + "/" + IFileService.StreamFile + "/{container}/{*blobName}")]
  [OutputCache(Duration = OneYearInSeconds)]
  [ResponseCache(Duration = OneYearInSeconds)]
  public async Task<IActionResult> StreamFile(string container, string blobName)
  {
    var file = await _fileService.ReadFile(container, blobName);
    if (file == null)
    {
      //return NotFound();
      return RedirectPermanent("https://gordonbeeming.com/404-file.png");
    }
    HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=31536000";
    return File(file.Binary, file.ContentType);
  }
}
