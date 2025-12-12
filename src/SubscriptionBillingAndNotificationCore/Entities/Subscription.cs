using SubscriptionBillingAndNotificationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class Subscription : BaseEntity
    {
        public string Type { get; set; }
        public decimal Pricing { get; set; }
        public SubscriptionFrequency Frequency { get; set; }
    }
}
