using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Areas.Blog.Data;

public partial class Categories
{
  [Key]
  public Guid Id { get; set; }

  [StringLength(128)]
  public string CategoryName { get; set; } = null!;

  [StringLength(128)]
  public string CategorySlug { get; set; } = null!;

  public byte DisplayIndex { get; set; }

  [StringLength(6)]
  [Unicode(false)]
  public string HexColour { get; set; } = null!;

  public DateTime DateTimeStamp { get; set; }

  public DateTime? CancelledDate { get; set; }

  [InverseProperty("Category")]
  public virtual ICollection<Blogs> Blogs { get; } = new List<Blogs>();
}
