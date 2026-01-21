using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructre.Migrations
{
    /// <inheritdoc />
    public partial class EditServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "AvailableTo",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableTo",
                table: "Services",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AvailableTo",
                table: "Services");

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "ServiceProviders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableTo",
                table: "ServiceProviders",
                type: "datetime2",
                nullable: true);
        }
    }
}
