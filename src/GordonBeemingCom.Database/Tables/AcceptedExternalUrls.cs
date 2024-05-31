using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Database.Tables;

public partial class AcceptedExternalUrls
{
  [Key] [StringLength(40)] public required string UrlHash { get; set; }

  [Required] [StringLength(2048)] public required string Url { get; set; }

  [Required] public DateTime DateTimeStamp { get; set; }

  public DateTime? CancelledDate { get; set; }

  [StringLength(50)] public string? DisableReason { get; set; }

  [Required] public required DateTime LastCheckedDate { get; set; }
  [Required] public required int ErrorCount { get; set; }
  [Required] public required int HttpStatusCode { get; set; }

  // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
  [Required] public required string Headers { get; set; } = default!;

  [Required] public bool IsSuccessStatusCode { get; set; }
}
