using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class UserSubscription : BaseEntity
    {
        public long UserId { get; set; }
        public long SubscriptionPlanId { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NextBillingDate { get; set; }
        public bool CancelAtExpiry { get; set; } = false;
        public DateTime? CancelledAt { get; set; }
        public SubCancellationReason CancellationReason { get; set; }
        public bool AutoRenew { get; set; } = true;
        public bool AdvanceReminderSent { get; set; }
        public bool ExpiryDayReminderSent { get; set; }


        // Navigation properties
        public User User { get; set; }
        public Subscription SubscriptionPlan { get; set; }
    }
}
