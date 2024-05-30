namespace GordonBeemingCom;

public static class GlobalConstants
{
  public const string BlogArea = "blog";

  public const string HomeControllerName = "Home";
  public const string BlogControllerName = "Blog";
  public const string ViewPostControllerName = "ViewPost";
  public const string TagPostsControllerName = "TagPosts";


  public const string IndexAction = "Index";

  public const string ProductionUrl = "https://gordonbeeming.com";
  #if DEBUG
  public const string PreviewUrl = "https://localhost:7054";
  #else
  public const string PreviewUrl = "https://preview.gordonbeeming.com";
  #endif
  public const string ProductionBlogUrl = ProductionUrl + "/" + BlogArea + "/";
  public const string BlogHomeImageRelativePath = "/images/blog-banner.jpg";
  public const string ProfileHomeImageRelativePath = "/images/profile-banner.jpg";
  public const string ProductionHomeImageUrl = ProductionUrl + BlogHomeImageRelativePath;

  public const string ProfileImageUrl = "https://en.gravatar.com/userimage/44277051/f0a721c3a3f76d28a9646a13c27eb7a7.jpeg";

  public const string BlogSubHeading = "Father | Husband | Triathlete | SSW Solution Architect | Microsoft MVP";

  public const string ProductionInstance = nameof(ProductionInstance);

  public const int OneDayInSeconds = 86400;
  public const int OneYearInSeconds = 31536000;

  #region Random App Areas

  public const string EmptyGuidArea = "empty-guid";
  public const string EmptyGuidArea_StaticFiles_RelativePath = "/apps/empty-guid/";

  #endregion
}
