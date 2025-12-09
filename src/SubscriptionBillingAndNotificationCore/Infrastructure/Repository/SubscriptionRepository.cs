using Microsoft.EntityFrameworkCore;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Repository
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SubscriptionRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Subscription> AddSubscription(Subscription subscription)
        {
            _dbContext.Subscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync();

            return subscription;
        }

        public async Task<Subscription> UpdateSubscription(Subscription subscription)
        {
            var existingSub = await GetSubscription(subscription.Id);
            if (existingSub == null)
                return subscription;

            _dbContext.Subscriptions.Update(subscription);
            await _dbContext.SaveChangesAsync();

            return existingSub;
        }

        public IEnumerable<Subscription> GetAllSubscriptions()
        {
            return _dbContext.Subscriptions.ToList().AsEnumerable();
        }

        public async Task<Subscription?> GetSubscription(long subscriptionId)
        {
            return await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == subscriptionId && x.IsDeleted == false);
        }

        
        public async Task<bool> DeleteSubscription(long subscriptionId)
        {
            var existingSubscription = await GetSubscription(subscriptionId);
            if (existingSubscription == null)
                return false;

            _dbContext.Subscriptions.Remove(existingSubscription);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public List<Subscription> SearchSubscriptions(string type)
        {
            List<Subscription> result = _dbContext.Subscriptions.Where(x => x.Type == type && !x.IsDeleted).ToList();

            return result;
        }
    }
}
