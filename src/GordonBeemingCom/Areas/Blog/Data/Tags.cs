using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Areas.Blog.Data;

public partial class Tags
{
  [Key]
  public Guid Id { get; set; }

  [StringLength(128)]
  public string TagName { get; set; } = null!;

  [StringLength(128)]
  public string TagSlug { get; set; } = null!;

  public DateTime DateTimeStamp { get; set; }

  public DateTime? CancelledDate { get; set; }

  [InverseProperty("Tag")]
  public virtual ICollection<BlogTags> BlogTags { get; } = new List<BlogTags>();
}
