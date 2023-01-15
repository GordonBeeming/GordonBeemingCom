namespace GordonBeemingCom.Areas.Blog.ViewModels;

public sealed class LandingPageViewModel
{
#pragma warning disable CS8618
  public List<Post> RecentPosts { get; set; } = new List<Post>();

  public sealed class Post
  {
    public Guid Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
  }

  public sealed class Tag
  {
    public string Name { get; set; }
    public string Slug { get; set; }
  }
}
