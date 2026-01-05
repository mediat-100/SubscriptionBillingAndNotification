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
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<BaseResponse<SubscriptionResponseDto>> AddSubscription(AddSubscriptionRequestDto subscription, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subscription.Type))
                throw new ValidationException("Please input the subscription type");

            if (subscription.Pricing <= 0)
                throw new ValidationException("Please input the subscription price");

            if ((int)subscription.Frequency < 1 || (int)subscription.Frequency > 3)
                throw new ValidationException("Please input the frequency of the subscription");

            var existingSubscription = _subscriptionRepository.SearchSubscriptions(subscription.Type);

            if (existingSubscription.Count() > 0)
                throw new ValidationException("Subscription Type already exist!");

            var newSubscription = ConvertToSubscriptionEntity(subscription);

            await _subscriptionRepository.AddSubscription(newSubscription, cancellationToken);

            var response = ConvertToSubscriptionResponseDto(newSubscription);

            return BaseResponse<SubscriptionResponseDto>.Ok(response, "Subscription Created Successfully"); 
        }

        public async Task<BaseResponse<SubscriptionResponseDto>> GetSubscriptionById(long id, CancellationToken cancellationToken)
        { 
            var subscription = await _subscriptionRepository.GetSubscription(id, cancellationToken)
                ?? throw new NotFoundException("SubscriptionId Not Found!");

            var response = ConvertToSubscriptionResponseDto(subscription);

            return BaseResponse<SubscriptionResponseDto>.Ok(response);
        }

        public async Task<BaseResponse<SubscriptionResponseDto>> UpdateSubscription(UpdateSubscriptionRequestDto subscription, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subscription.Type))
                throw new ValidationException("Please input the subscription type");

            if (subscription.Pricing <= 0)
                throw new ValidationException("Please input the subscription price");

            if ((int)subscription.Frequency < 1 || (int)subscription.Frequency > 3)
                throw new ValidationException("Please input the frequency of the subscription");

            var existingSubscription = await GetSubscriptionById(subscription.Id, cancellationToken);

            var subscriptionEntity = ConvertToSubscriptionEntity(subscription);

            var updatedSubscription = await _subscriptionRepository.UpdateSubscription(subscriptionEntity, cancellationToken);

            var response = ConvertToSubscriptionResponseDto(updatedSubscription);

            return BaseResponse<SubscriptionResponseDto>.Ok(response, "Subscription Updated Successfully");
        }
       

        public BaseResponse<IEnumerable<SubscriptionResponseDto>> SearchSubscriptions(string? type, int pageNumber = 10, int pageSize = 1)
        {
            var subscriptions = _subscriptionRepository.SearchSubscriptions(type, pageNumber, pageSize);
            IEnumerable<SubscriptionResponseDto> response = subscriptions.Select(x => ConvertToSubscriptionResponseDto(x));

            return BaseResponse<IEnumerable<SubscriptionResponseDto>>.Ok(response, "Fetched Subscriptions Successfully");
        }

        public async Task<BaseResponse<string>> DeleteSubscription(long id, CancellationToken cancellationToken)
        {
            var subscription = await GetSubscriptionById(id, cancellationToken);
            var isDeleted = await _subscriptionRepository.DeleteSubscription(id, cancellationToken);
            
            if (!isDeleted)
                throw new Exception("Failed To Delete Subscription, Please Try Again Later!");

            return BaseResponse<string>.Ok("", "Subscription Deleted Successfully");
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
                Id = subscription.Id,
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
                Id = subscription.Id,
                Type = subscription.Type,
                Pricing = subscription.Pricing,
                Frequency = subscription.Frequency.ToString(),
            };

            return response;
        }
    }
}
