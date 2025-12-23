using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class UserSubscription : BaseEntity
    {
        public long UserId { get; set; }
        public long SubscriptionPlanId { get; set; }
        public SubscriptionStatus SubscriptionStatus { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }


        // Navigation properties
        public User User { get; set; }
        public Subscription SubscriptionPlan { get; set; }
    }
}
