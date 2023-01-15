using Azure.Storage.Blobs;

namespace GordonBeemingCom.Areas.Blog.Services;

public sealed class BlobServiceClientService : IBlobServiceClientService
{
  private readonly IConfiguration _configuration;

  public BlobServiceClientService(IConfiguration configuration)
  {
    this._configuration = configuration;
  }

  public BlobServiceClient GetBlobServiceClient() => new BlobServiceClient(_configuration.GetConnectionString("BlobStorage"));
}
