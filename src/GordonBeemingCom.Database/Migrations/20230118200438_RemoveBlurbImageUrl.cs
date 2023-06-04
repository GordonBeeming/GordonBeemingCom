using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GordonBeemingCom.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBlurbImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlurbImageUrl",
                table: "Blogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlurbImageUrl",
                table: "Blogs",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");
        }
    }
}
