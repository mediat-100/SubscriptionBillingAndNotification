using SubscriptionBillingAndNotificationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Requests
{
    public class AddSubscriptionRequestDto
    {
        public string Type { get; set; }
        public decimal Pricing { get; set; }
        public SubscriptionFrequency Frequency { get; set; }
    }
}
