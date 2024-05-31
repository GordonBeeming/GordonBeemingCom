using System.Net;
using System.Net.Http.Headers;
using GordonBeemingCom.Shared.Models;
using GordonBeemingCom.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace GordonBeemingCom.Editor.Endpoints;

public static class ExternalUrlsServiceExtensions
{
  public static void RegisterUpdateDeadLinksEndpoint(this WebApplication app)
  {
    app.MapPost("/api/test", ()=> Results.Ok("Hello World"))
      .AllowAnonymous();
    app.MapPost("/api/update-dead-links", UpdateDeadLinks.EndPoint)
      .AllowAnonymous();
  }
}

public sealed class UpdateDeadLinks
{
  public static async Task<IResult> EndPoint(IExternalUrlsService externalUrlsService, ILogger<UpdateDeadLinks> logger,
    IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
  {
    var httpClient = httpClientFactory.CreateClient("link-checker");
    var activeLinks = await externalUrlsService.GetActiveLinks(25);
    var linksUpdated = 0;
    await Parallel.ForEachAsync(activeLinks, cancellationToken, async (link, token) =>
    {
      try
      {
        // Use HEAD request to retrieve headers only
        var request = new HttpRequestMessage(HttpMethod.Head, link.Url);
        // Some sites... like unsplashed block bots, so we're pretend to be the Googlebot ??... surprisingly this works
        // https://unsplash.com/@gordonbeeming
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Googlebot", "1.0"));
        var response = await httpClient.SendAsync(request, token);

        if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.MethodNotAllowed or HttpStatusCode.TooManyRequests)
        {
          request = new HttpRequestMessage(HttpMethod.Get, link.Url);
          response = await httpClient.SendAsync(request, token);
        }

        using var enumerator = response.Headers.GetEnumerator();
        await externalUrlsService.UpdateLinkDetails(new ExternalLinkDetails
        {
          UrlHash = link.UrlHash,
          Url = link.Url,
          Headers = GetList(enumerator).ToList(),
          IsSuccessStatusCode = response.IsSuccessStatusCode,
          HttpStatusCode = (int)response.StatusCode,
        });
      }
      catch (Exception e)
      {
        logger.LogError(e, "Error updating link [{Hash}] {Link}", link.UrlHash, link.Url);
      }

      Interlocked.Increment(ref linksUpdated);
    });

    return Results.Ok(linksUpdated);
  }

  private static IEnumerable<KeyValuePair<string, List<string>>> GetList(
    IEnumerator<KeyValuePair<string, IEnumerable<string>>> enumerator)
  {
    while (enumerator.MoveNext())
    {
      yield return new KeyValuePair<string, List<string>>(enumerator.Current.Key, enumerator.Current.Value.ToList());
    }
  }
}
