using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using GordonBeemingCom.Database;
using GordonBeemingCom.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Shared.Services;

public interface IExternalUrlsService
{
  Task AddAcceptedExternalUrlAsync(string url);
  Task<string?> GetRegisteredUrlAsync(string url);
  Task RegisterUrlsAsync(string html);
  Task CommitChangesAsync();
  Task<List<string>> GetUrlsCacheAsync();
}

public sealed class ExternalUrlsService : IExternalUrlsService
{
  private readonly HashHelper _hashHelper;
  private readonly AppDbContext _context;
  private static readonly ConcurrentBag<string> _urlCache = new();

  public ExternalUrlsService(HashHelper hashHelper, AppDbContext context)
  {
    _hashHelper = hashHelper;
    _context = context;
  }

  public async Task AddAcceptedExternalUrlAsync(string url)
  {
    if (url.StartsWith(GlobalConstants.ProductionUrl, StringComparison.InvariantCultureIgnoreCase) ||
        url.StartsWith("https://localhost", StringComparison.InvariantCultureIgnoreCase))
    {
      return;
    }
    if (_urlCache.Contains(url))
    {
      return;
    }
    var urlHash = await _hashHelper.GetHashOfString(url, HashHelper.Algorithms.SHA1);
    var urlFromDb =await GetUrlForHashAsync(urlHash);
    if (urlFromDb is not null)
    {
      _urlCache.Add(urlFromDb);
      return;
    }
    var acceptedExternalUrl = new AcceptedExternalUrls
    {
      UrlHash = urlHash,
      Url = url,
    };
    _context.AcceptedExternalUrls.Add(acceptedExternalUrl);
    _urlCache.Add(url);
  }

  public async Task<string?> GetRegisteredUrlAsync(string url)
  {
    if (_urlCache.Contains(url))
    {
      return url;
    }
    var urlHash = await _hashHelper.GetHashOfString(url, HashHelper.Algorithms.SHA1);
    var urlFromDb = await GetUrlForHashAsync(urlHash);
    if (urlFromDb is not null)
    {
      _urlCache.Add(urlFromDb);
    }
    return urlFromDb;
  }

  private async Task<string?> GetUrlForHashAsync(string urlHash)
  {
    return (await _context.AcceptedExternalUrls.FirstOrDefaultAsync(x => x.UrlHash == urlHash))?.Url;
  }

  public async Task RegisterUrlsAsync(string html)
  {
    string pattern = @"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1";
    var matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);
    foreach (Match match in matches)
    {
      var url = match.Groups[2].Value;
      if (url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
      {
        await AddAcceptedExternalUrlAsync(url);
      }
      else if (url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase))
      {
        url = $"https://{url.Remove(0, 7)}";
        await AddAcceptedExternalUrlAsync(url);
      }
    }
  }

  public async Task CommitChangesAsync()
  {
    await _context.SaveChangesAsync();
  }

  public Task<List<string>> GetUrlsCacheAsync()
  {
    return Task.FromResult(_urlCache.ToList());
  }
}
