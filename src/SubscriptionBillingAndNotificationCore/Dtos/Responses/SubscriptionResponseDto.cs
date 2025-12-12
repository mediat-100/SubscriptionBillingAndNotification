using SubscriptionBillingAndNotificationCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Responses
{
    public class SubscriptionResponseDto
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public decimal Pricing { get; set; }
        public string Frequency { get; set; }
    }
}
