using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Areas.Blog.Data;

public partial class BlogsRedirectUrl
{
  [Key]
  public Guid Id { get; set; }

  public Guid BlogsId { get; set; }

  [StringLength(1000)]
  public string RawUrl { get; set; } = null!;

  [Column(TypeName = "datetime")]
  public DateTime DateTimeStamp { get; set; }

  [ForeignKey("BlogsId")]
  [InverseProperty("BlogsRedirectUrl")]
  public virtual Blogs Blogs { get; set; } = null!;
}
