using Microsoft.Extensions.Configuration.UserSecrets;

namespace GordonBeemingCom.Areas.Blog.Services;

public sealed class FileService : IFileService
{
  private readonly IBlobServiceClientService _blobServiceClientService;
  private readonly HashHelper _hashHelper;

  public FileService(IBlobServiceClientService blobServiceClientService, HashHelper hashHelper)
  {
    this._blobServiceClientService = blobServiceClientService;
    _hashHelper = hashHelper;
  }

  public async Task<FileInformation> WriteFile(string uniqueIdentifier, IFormFile uploadFile)
  {
    if (uploadFile == null)
    {
      return null!;
    }
    if (uniqueIdentifier == null)
    {
      uniqueIdentifier = _hashHelper.GetHashFromStream(uploadFile.OpenReadStream(), HashHelper.Algorithms.SHA1);
    }
    var result = new FileInformation();
    result.UniqueIdentifier = uniqueIdentifier;
    result.Container = uploadFile.ContentType.Replace("/", "-").ToLowerInvariant();
    result.FileName = Path.GetFileName(uploadFile.FileName).ToLowerInvariant();
    result.BlobName = $"{uniqueIdentifier.ToLowerInvariant()}/{result.FileName}";
    result.RawUrl = $"/{BlogArea}/{result.Container}/{result.BlobName}";
    result.ContentType = uploadFile.ContentType;

    var blobServiceClient = _blobServiceClientService.GetBlobServiceClient();
    var containerClient = blobServiceClient.GetBlobContainerClient(result.Container);
    await containerClient.CreateIfNotExistsAsync();
    var blobClient = containerClient.GetBlobClient(result.BlobName);

    await blobClient.UploadAsync(uploadFile.OpenReadStream(), true);

    return result;
  }

  public async Task<FileInformation> WriteFile(string uniqueIdentifier, string fileName, string contentType, byte[] bytes)
  {
    if (uniqueIdentifier == null)
    {
      uniqueIdentifier = _hashHelper.GetHashFromBytes(bytes, HashHelper.Algorithms.SHA1);
    }
    var result = new FileInformation();
    result.UniqueIdentifier = uniqueIdentifier;
    result.Container = contentType.Replace("/", "-").ToLowerInvariant();
    result.FileName = Path.GetFileName(fileName).ToLowerInvariant();
    result.BlobName = $"{uniqueIdentifier.ToLowerInvariant()}/{result.FileName}";
    result.RawUrl = $"/{BlogArea}/{result.Container}/{result.BlobName}";
    result.ContentType = contentType;

    var blobServiceClient = _blobServiceClientService.GetBlobServiceClient();
    var containerClient = blobServiceClient.GetBlobContainerClient(result.Container);
    await containerClient.CreateIfNotExistsAsync();
    var blobClient = containerClient.GetBlobClient(result.BlobName);

    await blobClient.UploadAsync(new BinaryData(bytes), true);

    return result;
  }

  public async Task<FileInformation> ReadFile(string container, string blobName)
  {
    if (container == null || blobName == null)
    {
      return null!;
    }
    var result = new FileInformation();
    result.Container = container.ToLowerInvariant();
    result.BlobName = blobName.ToLowerInvariant();
    result.FileName = Path.GetFileName(blobName).ToLowerInvariant();
    result.UniqueIdentifier = result.BlobName.Remove(result.BlobName.LastIndexOf(result.FileName)).Trim('/');
    result.RawUrl = $"/{BlogArea}/{result.Container}/{result.BlobName}";
    result.ContentType = result.Container.Replace("-", "/");

    var blobServiceClient = _blobServiceClientService.GetBlobServiceClient();
    var containerClient = blobServiceClient.GetBlobContainerClient(result.Container);
    await containerClient.CreateIfNotExistsAsync();
    var blobClient = containerClient.GetBlobClient(result.BlobName);

    using (var ms = new MemoryStream())
    {
      await blobClient.DownloadToAsync(ms);
      result.Binary = ms.ToArray();
    }

    return result;
  }
}
