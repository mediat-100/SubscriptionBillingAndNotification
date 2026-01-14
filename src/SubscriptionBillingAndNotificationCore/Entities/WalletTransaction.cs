using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using SubscriptionBillingAndNotificationCore.Enums;

namespace SubscriptionBillingAndNotificationCore.Entities
{
    public class WalletTransaction : BaseEntity
    {
        public long WalletId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public Enums.TransactionStatus Status { get; set; }
        public string? Description { get; set; }
        public string ReferenceId { get; set; }
        public decimal BalanceAfter { get; set; }
        public Wallet Wallet { get; set; }
    }
}
