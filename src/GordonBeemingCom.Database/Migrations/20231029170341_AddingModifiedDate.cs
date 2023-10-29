using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GordonBeemingCom.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddingModifiedDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Blogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "(sysutcdatetime())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Blogs");
        }
    }
}
