using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionBillingAndNotificationCore.Migrations
{
    /// <inheritdoc />
    public partial class AddedReminderSentColToUserSubscriptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderSent",
                table: "UserSubscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderSent",
                table: "UserSubscriptions");
        }
    }
}
