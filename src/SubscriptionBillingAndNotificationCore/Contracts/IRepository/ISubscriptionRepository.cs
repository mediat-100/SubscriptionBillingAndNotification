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
        Task<Subscription> AddSubscription(Subscription subscription, CancellationToken cancellationToken);
        Task<Subscription> UpdateSubscription(Subscription subscription, CancellationToken cancellationToken);
        Task<Subscription?> GetSubscription(long subscriptionId, CancellationToken cancellationToken);
        Task<bool> DeleteSubscription(long subscriptionId, CancellationToken cancellationToken);
        IEnumerable<Subscription> SearchSubscriptions(string? type, int? isDeleted = 0, int pageNumber = 1, int pageSize = 10);
    }
}
