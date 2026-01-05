using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        public async Task<UserSubscription> AddUserSubscription(UserSubscription userSubscription, CancellationToken cancellationToken)
        {
            await _dbContext.UserSubscriptions.AddAsync(userSubscription, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

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

        public async Task<UserSubscription> UpdateUserSubscription(UserSubscription userSubscription, CancellationToken cancellationToken)
        {
            _dbContext.UserSubscriptions.Update(userSubscription);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return userSubscription;
        }

        public async Task<List<UserSubscription>> GetSubscriptionsExpiringIn3days(CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow.Date;
            var subscriptionsExpiringIn3days = await _dbContext.UserSubscriptions.Where(x => currentDate.AddDays(3) == x.EndDate.Date).ToListAsync(cancellationToken);

            return subscriptionsExpiringIn3days;
        }

        public async Task<List<UserSubscription>> GetExpiredSubscriptions(CancellationToken cancellationToken)
        {
            var currentDate = DateTime.UtcNow;
            var expiredSubscriptions = await _dbContext.UserSubscriptions.Where(x => (currentDate > x.EndDate || (int)x.SubscriptionStatus == 3) && !x.ExpiryDayReminderSent).Include(x => x.User).Include(x => x.SubscriptionPlan).ToListAsync(cancellationToken);

            return expiredSubscriptions;
        }
    }
}
