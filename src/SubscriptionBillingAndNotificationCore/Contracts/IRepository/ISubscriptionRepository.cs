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
        IEnumerable<Subscription> GetAllSubscriptions();
        Task<bool> DeleteSubscription(long subscriptionId);
        List<Subscription> SearchSubscriptions(string type);
    }
}
