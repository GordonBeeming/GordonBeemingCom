namespace GordonBeemingCom.Areas.Blog.ViewModels;

public sealed class TagPostsViewModel
{
#pragma warning disable CS8618
  public List<Post> TagPosts { get; set; } = new List<Post>();
  public string Title { get; set; }
  public string SubTitle { get; set; }
  public string BannerImage { get; set; }

  public sealed class Post
  {
    public Guid Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public DateTime? PublishDate { get; set; }

    public bool AddHR { get; set; } = true;
    public List<Tag> Tags { get; set; } = new List<Tag>();
  }

  public sealed class Tag
  {
    public string Name { get; set; }
    public string Slug { get; set; }
  }
}
