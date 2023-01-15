namespace GordonBeemingCom.Areas.Blog.ViewModels;

public sealed class RecentPostsViewModel
{
#pragma warning disable CS8618
  public List<Post> RecentPosts { get; set; } = new List<Post>();
  public Guid? NextPostId { get; set; }

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
