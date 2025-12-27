using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Responses
{
    public class UserSubscriptionResponseDto
    {
        public long UserId { get; set; }
        public long SubsciptionId { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime SubscriptionExpiryDateTime { get; set; }
    }
}
