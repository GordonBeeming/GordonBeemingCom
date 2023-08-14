namespace GordonBeemingCom.Shared.Services;

public interface IFileService
{
  const string DownloadFile = "binaries/files";
  const string StreamFile = "binaries/stream";

  Task<FileInformation> WriteFile(string? uniqueIdentifier, string fileName, string contentType, Func<Stream> openReadStream);
  Task<FileInformation> WriteFile(string? uniqueIdentifier, string fileName, string contentType, byte[] bytes);

  Task<FileInformation> ReadFile(string container, string blobName);
}

public sealed class FileService : IFileService
{
  private readonly IBlobServiceClientService _blobServiceClientService;
  private readonly HashHelper _hashHelper;

  public FileService(IBlobServiceClientService blobServiceClientService, HashHelper hashHelper)
  {
    this._blobServiceClientService = blobServiceClientService;
    _hashHelper = hashHelper;
  }

  public async Task<FileInformation> WriteFile(string? uniqueIdentifier, string fileName, string contentType, Func<Stream> openReadStream)
  {
    if (uniqueIdentifier == null)
    {
      uniqueIdentifier = await _hashHelper.GetHashFromStream(openReadStream(), HashHelper.Algorithms.SHA1);
    }
    var result = new FileInformation();
    result.UniqueIdentifier = uniqueIdentifier;
    result.Container = contentType.Replace("/", "-").ToLowerInvariant();
    result.FileName = Path.GetFileName(fileName).ToLowerInvariant();
    result.BlobName = $"{uniqueIdentifier.ToLowerInvariant()}/{result.FileName}";
    result.RawUrl = $"/{BlogArea}/{IFileService.StreamFile}/{result.Container}/{result.BlobName}";
    result.ContentType = contentType;

    var blobServiceClient = _blobServiceClientService.GetBlobServiceClient();
    var containerClient = blobServiceClient.GetBlobContainerClient(result.Container);
    await containerClient.CreateIfNotExistsAsync();
    var blobClient = containerClient.GetBlobClient(result.BlobName);

    await blobClient.UploadAsync(openReadStream(), true);

    return result;
  }

  public async Task<FileInformation> WriteFile(string? uniqueIdentifier, string fileName, string contentType, byte[] bytes)
  {
    if (uniqueIdentifier == null)
    {
      uniqueIdentifier = await _hashHelper.GetHashFromBytes(bytes, HashHelper.Algorithms.SHA1);
    }
    var result = new FileInformation();
    result.UniqueIdentifier = uniqueIdentifier;
    result.Container = contentType.Replace("/", "-").ToLowerInvariant();
    result.FileName = Path.GetFileName(fileName).ToLowerInvariant();
    result.BlobName = $"{uniqueIdentifier.ToLowerInvariant()}/{result.FileName}";
    result.RawUrl = $"/{BlogArea}/{IFileService.StreamFile}/{result.Container}/{result.BlobName}";
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
