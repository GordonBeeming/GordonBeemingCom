using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GordonBeemingCom.Database.Migrations
{
    /// <inheritdoc />
    public partial class DropPreAndPostSpacers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddPostSpacer",
                table: "BlogContentBlocks");

            migrationBuilder.DropColumn(
                name: "AddPreSpacer",
                table: "BlogContentBlocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AddPostSpacer",
                table: "BlogContentBlocks",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<bool>(
                name: "AddPreSpacer",
                table: "BlogContentBlocks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
