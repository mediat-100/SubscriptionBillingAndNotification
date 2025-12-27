using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Enums
{
    public enum SubscriptionStatus
    {
        Inactive = 0,
        Active = 1,
        Cancelled = 2,
        Expired = 3,
        Suspended = 4
    }
}
