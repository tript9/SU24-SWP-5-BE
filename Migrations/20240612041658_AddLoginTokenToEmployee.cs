using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWPApp.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginTokenToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginToken",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginTokenExpires",
                table: "Employees",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginToken",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LoginTokenExpires",
                table: "Employees");
        }
    }
}
