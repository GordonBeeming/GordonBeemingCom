using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Database.Tables;

public partial class Blogs
{
  [Key]
  public Guid Id { get; set; }

  public Guid CategoryId { get; set; }

  [StringLength(256)]
  public string BlogTitle { get; set; } = null!;

  [StringLength(256)]
  public string BlogSlug { get; set; } = null!;

  public string SubTitle { get; set; } = null!;
  
  [StringLength(1024)]
  public string HeroImageUrl { get; set; } = null!;

  public DateTime DateTimeStamp { get; set; }

  [Column(TypeName = "datetime")]
  public DateTime? PublishDate { get; set; }

  public DateTime? CancelledDate { get; set; }

  [StringLength(20)]
  [Unicode(false)]
  public string? YouTubeVideoId { get; set; }

  [InverseProperty("Blog")]
  public virtual ICollection<BlogContentBlocks> BlogContentBlocks { get; } = new List<BlogContentBlocks>();

  [InverseProperty("Blog")]
  public virtual ICollection<BlogTags> BlogTags { get; } = new List<BlogTags>();

  [InverseProperty("Blogs")]
  public virtual ICollection<BlogsRedirectUrl> BlogsRedirectUrl { get; } = new List<BlogsRedirectUrl>();

  [ForeignKey("CategoryId")]
  [InverseProperty("Blogs")]
  public virtual Categories Category { get; set; } = null!;
}
