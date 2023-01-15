using GordonBeemingCom.Areas.Blog.Models;

namespace GordonBeemingCom.Areas.Blog.Services;

public interface IFileService
{
  const string DownloadFile = "binaries/files";
  const string StreamFile = "binaries/stream";

  Task<FileInformation> WriteFile(string uniqueIdentifier, IFormFile uploadFile);
  Task<FileInformation> WriteFile(string uniqueIdentifier, string fileName, string contentType, byte[] bytes);

  Task<FileInformation> ReadFile(string container, string blobName);
}
