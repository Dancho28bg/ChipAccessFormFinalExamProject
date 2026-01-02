using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChipAccess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReassignmentAcceptance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "ReassignmentQueue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptedByNewUser",
                table: "ReassignmentQueue",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "ReassignmentQueue");

            migrationBuilder.DropColumn(
                name: "AcceptedByNewUser",
                table: "ReassignmentQueue");
        }
    }
}
