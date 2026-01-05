using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionBillingAndNotificationCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColsInUserSubscriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReminderSent",
                table: "UserSubscriptions",
                newName: "ExpiryDayReminderSent");

            migrationBuilder.AddColumn<bool>(
                name: "AdvanceReminderSent",
                table: "UserSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvanceReminderSent",
                table: "UserSubscriptions");

            migrationBuilder.RenameColumn(
                name: "ExpiryDayReminderSent",
                table: "UserSubscriptions",
                newName: "ReminderSent");
        }
    }
}
