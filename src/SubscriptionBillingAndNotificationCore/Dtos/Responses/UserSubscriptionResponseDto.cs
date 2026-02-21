using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Entities;

namespace SubscriptionBillingAndNotificationCore.Dtos.Responses
{
    public class UserSubscriptionResponseDto
    {
        public long UserId { get; set; }
        public long SubscriptionPlanId { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime SubcriptionStartDateTime { get; set; }
        public DateTime SubscriptionExpiryDateTime { get; set; }
        public bool AutoRenew { get; set; }

        public string SubscriptionPlanText
        {
            get
            {
                return SubscriptionPlanId switch
                {
                    1 => Enums.SubscriptionFrequency.Monthly.ToString(),
                    2 => Enums.SubscriptionFrequency.Quarterly.ToString(),
                    3 => Enums.SubscriptionFrequency.Yearly.ToString(),
                    _ => string.Empty
                };

            }
        }
    }
}
