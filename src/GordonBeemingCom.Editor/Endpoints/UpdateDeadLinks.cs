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
  public static async Task<IResult> EndPoint(IServiceProvider serviceProvider, ILogger<UpdateDeadLinks> logger,
    IHttpClientFactory httpClientFactory, CancellationToken cancellationToken)
  {
    var httpClient = httpClientFactory.CreateClient("link-checker");
    var externalUrlsService = serviceProvider.GetRequiredService<IExternalUrlsService>();
    var activeLinks = await externalUrlsService.GetActiveLinks(100);
    var linksUpdated = 0;
    await Parallel.ForEachAsync(activeLinks, cancellationToken, async (link, token) =>
    {
      // Create a CancellationTokenSource for the timeout
      using var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
      try
      {
        // Use HEAD request to retrieve headers only
        var request = new HttpRequestMessage(HttpMethod.Head, link.Url);
        // Some sites... like unsplashed block bots, so we're pretend to be the Googlebot ??... surprisingly this works
        // https://unsplash.com/@gordonbeeming
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Googlebot", "1.0"));
        var response = await httpClient.SendAsync(request, timeoutTokenSource.Token);
        // Handle the response, but first check if it was cancelled
        if (timeoutTokenSource.IsCancellationRequested)
        {
          logger.LogWarning("Request timed out for link: {Link}", link.Url);
        }
        else if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.MethodNotAllowed or HttpStatusCode.TooManyRequests)
        {
          request = new HttpRequestMessage(HttpMethod.Get, link.Url);
          response = await httpClient.SendAsync(request, timeoutTokenSource.Token);
        }
        using var enumerator = response.Headers.GetEnumerator();
        var localExternalUrlsService = serviceProvider.GetRequiredService<IExternalUrlsService>();
        await localExternalUrlsService.UpdateLinkDetails(new ExternalLinkDetails
        {
          UrlHash = link.UrlHash,
          Url = link.Url,
          Headers = GetList(enumerator).ToList(),
          IsSuccessStatusCode = response.IsSuccessStatusCode,
          HttpStatusCode = (int)response.StatusCode,
        });
      }
      catch (OperationCanceledException ex) when (ex.CancellationToken == timeoutTokenSource.Token)
      {
        logger.LogWarning("Request timed out for link [{Hash}] {Link}", link.UrlHash, link.Url);
        var localExternalUrlsService = serviceProvider.GetRequiredService<IExternalUrlsService>();
        await localExternalUrlsService.UpdateLinkDetails(new ExternalLinkDetails
        {
          UrlHash = link.UrlHash,
          Url = link.Url,
          Headers = [],
          IsSuccessStatusCode = false,
          HttpStatusCode = 408,// Request Timeout
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
