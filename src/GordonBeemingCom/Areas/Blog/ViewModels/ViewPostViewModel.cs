using Newtonsoft.Json;

namespace GordonBeemingCom.Areas.Blog.ViewModels;

public sealed class ViewPostViewModel
{
#pragma warning disable CS8618
  public Guid Id { get; set; }
  public string Title { get; set; }
  public string SubTitle { get; set; }
  public string? HeroImageUrl { get; set; }
  public CategoryInfo Category { get; set; } = new CategoryInfo();
  public List<TagInfo> Tags { get; set; } = new List<TagInfo>();
  public List<ContentBlock> ContentBlocks { get; set; } = new List<ContentBlock>();

  public sealed class CategoryInfo
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
  }

  public sealed class TagInfo
  {
    public string Name { get; set; }
    public string Slug { get; set; }
  }

  public class ContentBlock
  {
    public ContentBlockTypes BlockType { get; set; }
    public string ContextInfo { get; set; }

    public bool AddPreSpacer { get; set; }
    public bool AddPostSpacer { get; set; }

    public HtmlContentBlockContext ContextInfoAsHtml => JsonConvert.DeserializeObject<HtmlContentBlockContext>(ContextInfo)!;
    public ImageContentBlockContext ContextInfoAsImage => JsonConvert.DeserializeObject<ImageContentBlockContext>(ContextInfo)!;
    public CodeContentBlockContext ContextInfoAsCode => JsonConvert.DeserializeObject<CodeContentBlockContext>(ContextInfo)!;

    public short DisplayOrder { get; set; }
    public Guid ContentId { get; internal set; }
  }
}
