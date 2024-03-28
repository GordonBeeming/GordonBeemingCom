using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Database.Tables;

public partial class AcceptedExternalUrls
{
  [Key]
  [StringLength(40)]
  public required string UrlHash { get; set; }

  [StringLength(2048)]
  public required string Url { get; set; }

  public DateTime DateTimeStamp { get; set; }

  public DateTime? CancelledDate { get; set; }
}
