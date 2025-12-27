using SubscriptionBillingAndNotificationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Contracts.IRepository
{
    public interface ISubscriptionRepository
    {
        Task<Subscription> AddSubscription(Subscription subscription);
        Task<Subscription> UpdateSubscription(Subscription subscription);
        Task<Subscription?> GetSubscription(long subscriptionId);
        Task<bool> DeleteSubscription(long subscriptionId);
        IEnumerable<Subscription> SearchSubscriptions(string? type, int? isDeleted = 0, int pageNumber = 1, int pageSize = 10);
    }
}
