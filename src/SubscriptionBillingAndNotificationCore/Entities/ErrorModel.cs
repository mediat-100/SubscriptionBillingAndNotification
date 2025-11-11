using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class ErrorModel
    {
        public string? Detail { get; set; }
        public int StatusCode { get; set; }
        public string? Title { get; set; }
    }
}
