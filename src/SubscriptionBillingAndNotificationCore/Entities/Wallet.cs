using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class Wallet : BaseEntity
    {
        public long UserId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(3)]
        public string Currency { get; set; } = "NGN";
        public WalletStatus Status { get; set; }
        public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }
}
