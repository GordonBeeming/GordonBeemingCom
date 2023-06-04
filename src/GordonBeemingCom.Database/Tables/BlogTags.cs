using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Database.Tables;

[PrimaryKey("BlogId", "TagId")]
public partial class BlogTags
{
  [Key]
  public Guid BlogId { get; set; }

  [Key]
  public Guid TagId { get; set; }

  public DateTime DateTimeStamp { get; set; }

  [ForeignKey("BlogId")]
  [InverseProperty("BlogTags")]
  public virtual Blogs Blog { get; set; } = null!;

  [ForeignKey("TagId")]
  [InverseProperty("BlogTags")]
  public virtual Tags Tag { get; set; } = null!;
}
