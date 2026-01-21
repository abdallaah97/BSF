using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructre.Migrations
{
    /// <inheritdoc />
    public partial class editAvailablityTile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AvailableTo",
                table: "Services");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "ServiceProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
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
    }
}
