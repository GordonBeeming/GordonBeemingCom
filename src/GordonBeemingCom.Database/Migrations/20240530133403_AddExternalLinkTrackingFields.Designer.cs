﻿// <auto-generated />
using System;
using GordonBeemingCom.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GordonBeemingCom.Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240530133403_AddExternalLinkTrackingFields")]
    partial class AddExternalLinkTrackingFields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0-preview.4.24267.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.AcceptedExternalUrls", b =>
                {
                    b.Property<string>("UrlHash")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<DateTime?>("CancelledDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<string>("DisableReason")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("ErrorCount")
                        .HasColumnType("int");

                    b.Property<string>("Headers")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HttpStatusCode")
                        .HasColumnType("int");

                    b.Property<bool>("IsSuccessStatusCode")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastCheckedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValue(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("nvarchar(2048)");

                    b.HasKey("UrlHash");

                    b.ToTable("AcceptedExternalUrls");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.BlogContentBlocks", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<int>("BlockType")
                        .HasColumnType("int");

                    b.Property<Guid>("BlogId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CancelledDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ContextInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<short>("DisplayOrder")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("BlogId");

                    b.ToTable("BlogContentBlocks");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.BlogTags", b =>
                {
                    b.Property<Guid>("BlogId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.HasKey("BlogId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("BlogTags");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Blogs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<string>("BlogSlug")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("BlogTitle")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime?>("CancelledDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<string>("HeroImageUrl")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<DateTime>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("PublishDate")
                        .HasColumnType("datetime");

                    b.Property<string>("SubTitle")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("YouTubeVideoId")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Blogs");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.BlogsRedirectUrl", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<Guid>("BlogsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("RawUrl")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("BlogsId");

                    b.ToTable("BlogsRedirectUrl");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Categories", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime?>("CancelledDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("CategorySlug")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<byte>("DisplayIndex")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValueSql("((254))");

                    b.Property<string>("HexColour")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(6)
                        .IsUnicode(false)
                        .HasColumnType("varchar(6)")
                        .HasDefaultValueSql("((333333))");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Tags", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(newid())");

                    b.Property<DateTime?>("CancelledDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("TagSlug")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.BlogContentBlocks", b =>
                {
                    b.HasOne("GordonBeemingCom.Database.Tables.Blogs", "Blog")
                        .WithMany("BlogContentBlocks")
                        .HasForeignKey("BlogId")
                        .IsRequired()
                        .HasConstraintName("FK_BlogContentBlocks_Blogs");

                    b.Navigation("Blog");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.BlogTags", b =>
                {
                    b.HasOne("GordonBeemingCom.Database.Tables.Blogs", "Blog")
                        .WithMany("BlogTags")
                        .HasForeignKey("BlogId")
                        .IsRequired()
                        .HasConstraintName("FK_BlogTags_Blogs");

                    b.HasOne("GordonBeemingCom.Database.Tables.Tags", "Tag")
                        .WithMany("BlogTags")
                        .HasForeignKey("TagId")
                        .IsRequired()
                        .HasConstraintName("FK_BlogTags_Tags");

                    b.Navigation("Blog");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Blogs", b =>
                {
                    b.HasOne("GordonBeemingCom.Database.Tables.Categories", "Category")
                        .WithMany("Blogs")
                        .HasForeignKey("CategoryId")
                        .IsRequired()
                        .HasConstraintName("FK_Blogs_Categories");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.BlogsRedirectUrl", b =>
                {
                    b.HasOne("GordonBeemingCom.Database.Tables.Blogs", "Blogs")
                        .WithMany("BlogsRedirectUrl")
                        .HasForeignKey("BlogsId")
                        .IsRequired()
                        .HasConstraintName("FK_BlogsRedirectUrl_Blogs");

                    b.Navigation("Blogs");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Blogs", b =>
                {
                    b.Navigation("BlogContentBlocks");

                    b.Navigation("BlogTags");

                    b.Navigation("BlogsRedirectUrl");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Categories", b =>
                {
                    b.Navigation("Blogs");
                });

            modelBuilder.Entity("GordonBeemingCom.Database.Tables.Tags", b =>
                {
                    b.Navigation("BlogTags");
                });
#pragma warning restore 612, 618
        }
    }
}
