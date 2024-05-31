using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GordonBeemingCom.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalLinkTrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisableReason",
                table: "AcceptedExternalUrls",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorCount",
                table: "AcceptedExternalUrls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Headers",
                table: "AcceptedExternalUrls",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HttpStatusCode",
                table: "AcceptedExternalUrls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuccessStatusCode",
                table: "AcceptedExternalUrls",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckedDate",
                table: "AcceptedExternalUrls",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisableReason",
                table: "AcceptedExternalUrls");

            migrationBuilder.DropColumn(
                name: "ErrorCount",
                table: "AcceptedExternalUrls");

            migrationBuilder.DropColumn(
                name: "Headers",
                table: "AcceptedExternalUrls");

            migrationBuilder.DropColumn(
                name: "HttpStatusCode",
                table: "AcceptedExternalUrls");

            migrationBuilder.DropColumn(
                name: "IsSuccessStatusCode",
                table: "AcceptedExternalUrls");

            migrationBuilder.DropColumn(
                name: "LastCheckedDate",
                table: "AcceptedExternalUrls");
        }
    }
}
