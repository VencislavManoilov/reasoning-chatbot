using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsoluteExpiration",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SlidingExpirationInSeconds",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ExpiresAtTime",
                table: "Sessions",
                newName: "ExpiresAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "Sessions",
                newName: "ExpiresAtTime");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AbsoluteExpiration",
                table: "Sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SlidingExpirationInSeconds",
                table: "Sessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
