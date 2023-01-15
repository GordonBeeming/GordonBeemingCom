using Azure.Storage.Blobs;

namespace GordonBeemingCom.Areas.Blog.Services;

public interface IBlobServiceClientService
{
  BlobServiceClient GetBlobServiceClient();
}
