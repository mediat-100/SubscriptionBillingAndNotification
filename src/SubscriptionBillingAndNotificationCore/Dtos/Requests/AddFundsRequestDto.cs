using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Dtos.Requests
{
    public class AddFundsRequestDto
    {
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
