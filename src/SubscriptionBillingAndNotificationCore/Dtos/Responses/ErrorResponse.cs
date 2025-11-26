using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public ErrorDetails? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ErrorDetails
    {
        public string? StackTrace { get; set; }
        public string? InnerException { get; set; }
        public string? OtherDetails { get; set; }
    }
}
