using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Areas.Blog.Data;

public partial class BlogContentBlocks
{
  [Key]
  public Guid Id { get; set; }

  public Guid BlogId { get; set; }

  public short BlockType { get; set; }

  public string ContextInfo { get; set; } = null!;

  public short DisplayOrder { get; set; }

  public bool AddPreSpacer { get; set; }

  [Required]
  public bool? AddPostSpacer { get; set; }

  public DateTime DateTimeStamp { get; set; }

  public DateTime? CancelledDate { get; set; }

  [ForeignKey("BlogId")]
  [InverseProperty("BlogContentBlocks")]
  public virtual Blogs Blog { get; set; } = null!;
}
