using Azure.Storage.Blobs;

namespace GordonBeemingCom.Shared.Services;

public interface IBlobServiceClientService
{
  BlobServiceClient GetBlobServiceClient();
}
