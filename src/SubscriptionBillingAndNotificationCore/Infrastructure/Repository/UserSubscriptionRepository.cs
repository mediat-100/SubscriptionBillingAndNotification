using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Repository
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserSubscriptionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<UserSubscription> AddUserSubscription(UserSubscription userSubscription)
        {
            await _dbContext.UserSubscriptions.AddAsync(userSubscription);
            await _dbContext.SaveChangesAsync();

            return userSubscription;
        }

        public UserSubscription? GetSubscriptionById(long userSubscriptionId)
        {
            var userSubscription = _dbContext.UserSubscriptions.Where(x => x.Id == userSubscriptionId && !x.IsDeleted).FirstOrDefault();
            if (userSubscription == null)
                return null;

            return userSubscription;
        }

        public IEnumerable<UserSubscription> SearchUserSubscriptions(long? userId, long? subscriptionId, int? subscriptionStatus, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<UserSubscription> query = _dbContext.UserSubscriptions;

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId);

            if (subscriptionId.HasValue)
                query = query.Where(x => x.SubscriptionPlanId == subscriptionId);

            if (subscriptionStatus.HasValue)
                query = query.Where(x => (int)x.SubscriptionStatus == subscriptionStatus);
            
            var result = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

            return result;
        }

        public IEnumerable<UserSubscription> GetActiveSubscriptions(int pageNumber = 1, int pageSize = 10)
        {
            var activeSubscriptions = _dbContext.UserSubscriptions.Where(x => x.SubscriptionStatus.ToString() == Enums.SubscriptionStatus.Active.ToString())
                                                                  .Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();
            return activeSubscriptions;
        }

        public async Task<UserSubscription> UpdateUserSubscription(UserSubscription userSubscription)
        {
            _dbContext.UserSubscriptions.Update(userSubscription);
            await _dbContext.SaveChangesAsync();

            return userSubscription;
        }
    }
}
