using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using GordonBeemingCom.Database;
using GordonBeemingCom.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Shared.Services;

public interface IExternalUrlsService
{
  Task AddAcceptedExternalUrlAsync(string url);
  Task<ExternalLinkDetails?> GetRegisteredUrlAsync(string url);
  Task RegisterUrlsAsync(string html);
  Task CommitChangesAsync();
  Task<List<ExternalLinkDetails>> GetUrlsCacheAsync();
  Task<List<ExternalLink>> GetActiveLinks(int? top = null);
  Task UpdateLinkDetails(ExternalLinkDetails externalLinkDetails);
  Task<ExternalLinkDetails?> GetRegisteredUrlByHashAsync(string hashId);
}

public sealed class ExternalUrlsService : IExternalUrlsService
{
  private readonly HashHelper _hashHelper;
  private readonly AppDbContext _context;
  private static readonly ConcurrentDictionary<string, ExternalLinkDetails> _urlCache = new();

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

    var urlHash = await _hashHelper.GetHashOfString(url, HashHelper.Algorithms.SHA1);
    if (_urlCache.ContainsKey(urlHash))
    {
      return;
    }

    var urlFromDb = await GetUrlForHashAsync(urlHash);
    if (urlFromDb is not null)
    {
      _urlCache.AddOrUpdate(urlHash, urlFromDb, (key, oldValue) => urlFromDb);
      return;
    }

    var acceptedExternalUrl = new AcceptedExternalUrls
    {
      UrlHash = urlHash,
      Url = url,
      Headers = "",
      LastCheckedDate = DateTime.Today.AddDays(-1000),
      IsSuccessStatusCode = true,
      DateTimeStamp = DateTime.UtcNow,
      ErrorCount = 0,
      HttpStatusCode = 200,
    };
    var link = new ExternalLinkDetails
    {
      Headers = [],
      Url = acceptedExternalUrl.Url,
      UrlHash = acceptedExternalUrl.UrlHash,
      HttpStatusCode = 0,
      IsSuccessStatusCode = true,
    };
    _context.AcceptedExternalUrls.Add(acceptedExternalUrl);
    _urlCache.AddOrUpdate(acceptedExternalUrl.UrlHash, link,
      (key, oldValue) => link);
  }

  public async Task<ExternalLinkDetails?> GetRegisteredUrlAsync(string url)
  {
    var urlHash = await _hashHelper.GetHashOfString(url, HashHelper.Algorithms.SHA1);

    if (_urlCache.TryGetValue(urlHash, out var cachedUrl))
    {
      return cachedUrl;
    }

    var urlFromDb = await GetUrlForHashAsync(urlHash);
    if (urlFromDb is not null)
    {
      _urlCache.AddOrUpdate(urlHash, urlFromDb, (key, oldValue) => urlFromDb);
    }

    return urlFromDb;
  }

  private async Task<ExternalLinkDetails?> GetUrlForHashAsync(string urlHash)
  {
    var link = await _context.AcceptedExternalUrls.FirstOrDefaultAsync(x => x.UrlHash == urlHash);
    if (link is null)
    {
      return null;
    }

    return new ExternalLinkDetails
    {
      Headers = link.Headers.Length > 0
        ? JsonConvert.DeserializeObject<List<KeyValuePair<string, List<string>>>>(link.Headers)!
        : [],
      Url = link.Url,
      UrlHash = link.UrlHash,
      HttpStatusCode = link.HttpStatusCode,
      IsSuccessStatusCode = link.IsSuccessStatusCode,
      DisableReason = link.DisableReason,
      FirstUsed = link.DateTimeStamp.Date,
      LastUsed = link.LastCheckedDate.Date < link.DateTimeStamp.Date ? link.DateTimeStamp.Date : link.LastCheckedDate.Date,
    };
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

  public Task<List<ExternalLinkDetails>> GetUrlsCacheAsync()
  {
    return Task.FromResult(_urlCache.Select(o => o.Value).ToList());
  }

  public async Task<List<ExternalLink>> GetActiveLinks(int? top = null)
  {
    var links = await _context.AcceptedExternalUrls
      .AsNoTracking()
      .Where(o => o.DisableReason == null && o.LastCheckedDate.AddDays(o.ErrorCount).AddDays(15) < DateTime.UtcNow)
      .Take(top ?? int.MaxValue)
      .Select(o => new ExternalLink { UrlHash = o.UrlHash, Url = o.Url, })
      .ToListAsync();

    return links;
  }

  public async Task UpdateLinkDetails(ExternalLinkDetails externalLinkDetails)
  {
    var link = await _context.AcceptedExternalUrls.FirstOrDefaultAsync(o => o.UrlHash == externalLinkDetails.UrlHash);
    if (link is null)
    {
      return;
    }

    link.HttpStatusCode = externalLinkDetails.HttpStatusCode;
    link.Headers = JsonConvert.SerializeObject(externalLinkDetails.Headers.OrderBy(o => o.Key));
    link.IsSuccessStatusCode = externalLinkDetails.IsSuccessStatusCode;
    if (!externalLinkDetails.IsSuccessStatusCode && link.HttpStatusCode != 999)
    {
      link.ErrorCount++;
      if (link.ErrorCount >= 5)
      {
        link.DisableReason = $"HTTP Status Code: {externalLinkDetails.HttpStatusCode}";
      }
    }
    else
    {
      link.LastCheckedDate = DateTime.UtcNow;
      link.DisableReason = null;
      link.ErrorCount = 0;
    }

    await _context.SaveChangesAsync();
  }

  public Task<ExternalLinkDetails?> GetRegisteredUrlByHashAsync(string hashId)
  {
    return GetUrlForHashAsync(hashId);
  }
}
