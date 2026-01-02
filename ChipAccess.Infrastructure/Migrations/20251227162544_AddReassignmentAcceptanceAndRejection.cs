using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChipAccess.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReassignmentAcceptanceAndRejection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "ReassignmentQueue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "ReassignmentQueue",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RejectedByNewUser",
                table: "ReassignmentQueue",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "ReassignmentQueue");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "ReassignmentQueue");

            migrationBuilder.DropColumn(
                name: "RejectedByNewUser",
                table: "ReassignmentQueue");
        }
    }
}
