using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionBillingAndNotificationCore.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColsToUserSubsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CancelAtExpiry",
                table: "UserSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CancellationReason",
                table: "UserSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "UserSubscriptions",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelAtExpiry",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "UserSubscriptions");
        }
    }
}
