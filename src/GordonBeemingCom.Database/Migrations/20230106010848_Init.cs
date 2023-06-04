using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GordonBeemingCom.Database.Migrations;

/// <inheritdoc />
public partial class Init : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "Categories",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
          CategoryName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
          CategorySlug = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
          DisplayIndex = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "((254))"),
          HexColour = table.Column<string>(type: "varchar(6)", unicode: false, maxLength: 6, nullable: false, defaultValueSql: "((333333))"),
          DateTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
          CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Categories", x => x.Id);
        });

    migrationBuilder.CreateTable(
        name: "Tags",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
          TagName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
          TagSlug = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
          DateTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
          CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Tags", x => x.Id);
        });

    migrationBuilder.CreateTable(
        name: "Blogs",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
          CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          BlogTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
          BlogSlug = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
          BlurbHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
          BlurbImageUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
          HeroImageUrl = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
          DateTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
          PublishDate = table.Column<DateTime>(type: "datetime", nullable: true),
          CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true),
          YouTubeVideoId = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Blogs", x => x.Id);
          table.ForeignKey(
              name: "FK_Blogs_Categories",
              column: x => x.CategoryId,
              principalTable: "Categories",
              principalColumn: "Id");
        });

    migrationBuilder.CreateTable(
        name: "BlogContentBlocks",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
          BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          BlockType = table.Column<short>(type: "smallint", nullable: false),
          ContextInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
          DisplayOrder = table.Column<short>(type: "smallint", nullable: false),
          AddPreSpacer = table.Column<bool>(type: "bit", nullable: false),
          AddPostSpacer = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
          DateTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
          CancelledDate = table.Column<DateTime>(type: "datetime2", nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_BlogContentBlocks", x => x.Id);
          table.ForeignKey(
              name: "FK_BlogContentBlocks_Blogs",
              column: x => x.BlogId,
              principalTable: "Blogs",
              principalColumn: "Id");
        });

    migrationBuilder.CreateTable(
        name: "BlogsRedirectUrl",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
          BlogsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          RawUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
          DateTimeStamp = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())")
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_BlogsRedirectUrl", x => x.Id);
          table.ForeignKey(
              name: "FK_BlogsRedirectUrl_Blogs",
              column: x => x.BlogsId,
              principalTable: "Blogs",
              principalColumn: "Id");
        });

    migrationBuilder.CreateTable(
        name: "BlogTags",
        columns: table => new
        {
          BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          DateTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())")
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_BlogTags", x => new { x.BlogId, x.TagId });
          table.ForeignKey(
              name: "FK_BlogTags_Blogs",
              column: x => x.BlogId,
              principalTable: "Blogs",
              principalColumn: "Id");
          table.ForeignKey(
              name: "FK_BlogTags_Tags",
              column: x => x.TagId,
              principalTable: "Tags",
              principalColumn: "Id");
        });

    migrationBuilder.CreateIndex(
        name: "IX_BlogContentBlocks_BlogId",
        table: "BlogContentBlocks",
        column: "BlogId");

    migrationBuilder.CreateIndex(
        name: "IX_Blogs_CategoryId",
        table: "Blogs",
        column: "CategoryId");

    migrationBuilder.CreateIndex(
        name: "IX_BlogsRedirectUrl_BlogsId",
        table: "BlogsRedirectUrl",
        column: "BlogsId");

    migrationBuilder.CreateIndex(
        name: "IX_BlogTags_TagId",
        table: "BlogTags",
        column: "TagId");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "BlogContentBlocks");

    migrationBuilder.DropTable(
        name: "BlogsRedirectUrl");

    migrationBuilder.DropTable(
        name: "BlogTags");

    migrationBuilder.DropTable(
        name: "Blogs");

    migrationBuilder.DropTable(
        name: "Tags");

    migrationBuilder.DropTable(
        name: "Categories");
  }
}
