using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Requests
{
    public class ActivateSubscriptionRequestDto
    {
        public long UserId { get; set; }
        public long SubsciptionPlanId { get; set; }
        public bool AutoRenew { get; set; }
    }
}
