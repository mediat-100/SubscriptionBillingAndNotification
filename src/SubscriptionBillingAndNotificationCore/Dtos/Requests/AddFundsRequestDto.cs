using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Dtos.Requests
{
    public class AddFundsRequestDto
    {
        public long WalletId { get; set; }
        public decimal Amount { get; set; }
    }
}
