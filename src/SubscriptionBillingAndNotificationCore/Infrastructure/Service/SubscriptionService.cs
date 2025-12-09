using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ApplicationDbContext dbContext, ISubscriptionRepository subscriptionRepository)
        {
            _dbContext = dbContext;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<BaseResponse<SubscriptionResponseDto>> AddSubscription(AddSubscriptionRequestDto subscription)
        {
            if (string.IsNullOrEmpty(subscription.Type))
                throw new ValidationException("Please input the subscription type");

            if (subscription.Pricing <= 0)
                throw new ValidationException("Please input the subscription price");

            if ((int)subscription.Frequency < 1 || (int)subscription.Frequency > 3)
                throw new ValidationException("Please input the frequency of the subscription");

            var existingSubscription = _subscriptionRepository.SearchSubscriptions(subscription.Type);

            if (existingSubscription.Count() > 0)
                throw new ValidationException("Subscription already exist!");

            var newSubscription = ConvertToSubscriptionEntity(subscription);

            await _subscriptionRepository.AddSubscription(newSubscription);

            var response = ConvertToSubscriptionResponseDto(newSubscription);

            return BaseResponse<SubscriptionResponseDto>.Ok(response, "Subscription Created Successfully"); 
        }

        public BaseResponse<IEnumerable<SubscriptionResponseDto>> GetAllSubscriptions()
        {
            var subscriptions = _subscriptionRepository.GetAllSubscriptions();

            var response = subscriptions.Select(x => ConvertToSubscriptionResponseDto(x));

            return BaseResponse<IEnumerable<SubscriptionResponseDto>>.Ok(response);

            // TODO: Implement pagination
        }

        public async Task<BaseResponse<SubscriptionResponseDto>> GetSubscriptionById(long id)
        {
            var subscription = await _subscriptionRepository.GetSubscription(id)
                ?? throw new NotFoundException("SubscriptionId Not Found!");

            var response = ConvertToSubscriptionResponseDto(subscription);

            return BaseResponse<SubscriptionResponseDto>.Ok(response);
        }

        public async Task<BaseResponse<SubscriptionResponseDto>> UpdateSubscription(UpdateSubscriptionRequestDto subscription)
        {
            if (string.IsNullOrEmpty(subscription.Type))
                throw new ValidationException("Please input the subscription type");

            if (subscription.Pricing <= 0)
                throw new ValidationException("Please input the subscription price");

            if ((int)subscription.Frequency < 1 || (int)subscription.Frequency > 3)
                throw new ValidationException("Please input the frequency of the subscription");

            var existingSubscription = GetSubscriptionById(subscription.Id);

            var subscriptionEntity = ConvertToSubscriptionEntity(subscription);

            var updatedSubscription = await _subscriptionRepository.UpdateSubscription(subscriptionEntity);

            var response = ConvertToSubscriptionResponseDto(updatedSubscription);

            return BaseResponse<SubscriptionResponseDto>.Ok(response, "Subscription Updated Successfully");
        }
       

        public BaseResponse<IEnumerable<SubscriptionResponseDto>> SearchSubscriptions(string type)
        {
            var subscriptions = _subscriptionRepository.SearchSubscriptions(type);
            IEnumerable<SubscriptionResponseDto> response = subscriptions.Select(x => ConvertToSubscriptionResponseDto(x));

            return BaseResponse<IEnumerable<SubscriptionResponseDto>>.Ok(response);
        }

        public async Task<BaseResponse<string>> DeleteSubscription(long id)
        {
            var subscription = await GetSubscriptionById(id);
            var isDeleted = await _subscriptionRepository.DeleteSubscription(id);
            
            if (!isDeleted)
                return BaseResponse<string>.Fail("Failed To Delete Subscription, Please Try Again Later!");

            return BaseResponse<string>.Ok("Subscription Deleted Successfully", "Subscription Deleted Successfully");
        }






        private Subscription ConvertToSubscriptionEntity(AddSubscriptionRequestDto subscription)
        {
            var response = new Subscription()
            {
                Type = subscription.Type,
                Pricing = subscription.Pricing,
                Frequency = subscription.Frequency,
            };

            return response;
        }

        private Subscription ConvertToSubscriptionEntity(UpdateSubscriptionRequestDto subscription)
        {
            var response = new Subscription()
            {
                Type = subscription.Type,
                Pricing = subscription.Pricing,
                Frequency = subscription.Frequency,
            };

            return response;
        }

        private SubscriptionResponseDto ConvertToSubscriptionResponseDto(Subscription subscription)
        {
            var response = new SubscriptionResponseDto
            {
                Type = subscription.Type,
                Pricing = subscription.Pricing,
                Frequency = subscription.Frequency,
            };

            return response;
        }
    }
}
