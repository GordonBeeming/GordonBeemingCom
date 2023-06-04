using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace GordonBeemingCom.Shared.Services;

public sealed class BlobServiceClientService : IBlobServiceClientService
{
  private readonly IConfiguration _configuration;

  public BlobServiceClientService(IConfiguration configuration)
  {
    this._configuration = configuration;
  }

  public BlobServiceClient GetBlobServiceClient()
  {
    var connectionString = _configuration.GetConnectionString("BlobStorage");
    if (!string.IsNullOrEmpty(connectionString))
    {
      return new BlobServiceClient(connectionString);
    }
    var blobStorageUrl = _configuration["BlobStorageUrl"];
    if (!string.IsNullOrEmpty(blobStorageUrl))
    {
      return new BlobServiceClient(new Uri(blobStorageUrl), new DefaultAzureCredential());
    }
    throw new Exception("Please configure ConnectionStrings:BlobStorage or BlobStorageUrl in appsettings.json");
  }
}
