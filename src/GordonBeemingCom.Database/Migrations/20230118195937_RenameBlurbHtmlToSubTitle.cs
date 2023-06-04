using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GordonBeemingCom.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameBlurbHtmlToSubTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlurbHtml",
                table: "Blogs",
                newName: "SubTitle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubTitle",
                table: "Blogs",
                newName: "BlurbHtml");
        }
    }
}
