using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Entities;

namespace SubscriptionBillingAndNotificationCore.Contracts.IRepository
{
    public interface IUserSubscriptionRepository
    {
        Task<UserSubscription> AddUserSubscription(UserSubscription userSubscription);
        UserSubscription GetSubscriptionById(long userSubscriptionId);
        IEnumerable<UserSubscription> SearchUserSubscriptions(long? userId = null, long? subscriptionId = null, int? subscriptionStatus = null, int pageNumber = 1, int pageSize = 10);
        IEnumerable<UserSubscription> GetActiveSubscriptions(int pageNumber = 1, int pageSize = 10);
        Task<UserSubscription> UpdateUserSubscription(UserSubscription userSubscription);
    }
}
