using GordonBeemingCom.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace GordonBeemingCom.Database;

public partial class AppDbContext : DbContext
{
#pragma warning disable CS8618
  public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
  {
  }

  public virtual DbSet<AcceptedExternalUrls> AcceptedExternalUrls { get; set; }

  public virtual DbSet<BlogContentBlocks> BlogContentBlocks { get; set; }

  public virtual DbSet<BlogTags> BlogTags { get; set; }

  public virtual DbSet<Blogs> Blogs { get; set; }

  public virtual DbSet<BlogsRedirectUrl> BlogsRedirectUrl { get; set; }

  public virtual DbSet<Categories> Categories { get; set; }

  public virtual DbSet<Tags> Tags { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<AcceptedExternalUrls>(entity =>
    {
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(sysutcdatetime())");
      entity.Property(e => e.LastCheckedDate).HasDefaultValue(new DateTime(2000, 1, 1));

      entity.HasQueryFilter(b => EF.Property<DateTime?>(b, "CancelledDate") == null);
    });

    modelBuilder.Entity<BlogContentBlocks>(entity =>
    {
      entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(sysutcdatetime())");

      entity.HasOne(d => d.Blog).WithMany(p => p.BlogContentBlocks)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_BlogContentBlocks_Blogs");

      entity.HasQueryFilter(b => EF.Property<DateTime?>(b, "CancelledDate") == null);
    });

    modelBuilder.Entity<BlogTags>(entity =>
    {
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(sysutcdatetime())");

      entity.HasOne(d => d.Blog).WithMany(p => p.BlogTags)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_BlogTags_Blogs");

      entity.HasOne(d => d.Tag).WithMany(p => p.BlogTags)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_BlogTags_Tags");
    });

    modelBuilder.Entity<Blogs>(entity =>
    {
      entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(sysutcdatetime())");

      entity.HasOne(d => d.Category).WithMany(p => p.Blogs)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_Blogs_Categories");

      entity.HasQueryFilter(b => EF.Property<DateTime?>(b, "CancelledDate") == null);
    });

    modelBuilder.Entity<BlogsRedirectUrl>(entity =>
    {
      entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(getutcdate())");

      entity.HasOne(d => d.Blogs).WithMany(p => p.BlogsRedirectUrl)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK_BlogsRedirectUrl_Blogs");
    });

    modelBuilder.Entity<Categories>(entity =>
    {
      entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(sysutcdatetime())");
      entity.Property(e => e.DisplayIndex).HasDefaultValueSql("((254))");
      entity.Property(e => e.HexColour).HasDefaultValueSql("((333333))");

      entity.HasQueryFilter(b => EF.Property<DateTime?>(b, "CancelledDate") == null);
    });

    modelBuilder.Entity<Tags>(entity =>
    {
      entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
      entity.Property(e => e.DateTimeStamp).HasDefaultValueSql("(sysutcdatetime())");

      entity.HasQueryFilter(b => EF.Property<DateTime?>(b, "CancelledDate") == null);
    });
  }
}
