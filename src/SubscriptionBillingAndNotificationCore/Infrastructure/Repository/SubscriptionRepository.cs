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

        public async Task<Subscription> AddSubscription(Subscription subscription, CancellationToken cancellationToken)
        {
            _dbContext.Subscriptions.Add(subscription);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return subscription;
        }

        public async Task<Subscription> UpdateSubscription(Subscription subscription, CancellationToken cancellationToken)
        {
            var existingSub = await GetSubscription(subscription.Id, cancellationToken);
            if (existingSub == null)
                return subscription;

            existingSub.Frequency = subscription.Frequency;
            existingSub.Pricing = subscription.Pricing;
            existingSub.Type = subscription.Type;

            _dbContext.Subscriptions.Update(existingSub);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return subscription;
        }

        public async Task<Subscription?> GetSubscription(long subscriptionId, CancellationToken cancellationToken)
        {
            return await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == subscriptionId && !x.IsDeleted, cancellationToken);
        }

        
        public async Task<bool> DeleteSubscription(long subscriptionId, CancellationToken cancellationToken)
        {
            var existingSubscription = await GetSubscription(subscriptionId, cancellationToken);
            if (existingSubscription == null)
                return false;

            existingSubscription.IsDeleted = true;
            _dbContext.Subscriptions.Update(existingSubscription);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public IEnumerable<Subscription> SearchSubscriptions(string? type, int? isDeleted = 0, int pageNumber = 1, int pageSize = 10)
        {
            IQueryable<Subscription> query = _dbContext.Subscriptions;

            if (string.IsNullOrWhiteSpace(type))
                query = query.Where(x => x.Type == type);

            if (isDeleted.HasValue && isDeleted == 1)
                query = query.Where(x => x.IsDeleted);
           
            var result = query.Skip((pageNumber -1) * pageSize).Take(pageSize).AsEnumerable();

            return result;
        }
    }
}
