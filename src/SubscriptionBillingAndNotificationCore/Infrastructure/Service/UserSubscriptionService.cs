using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Enums;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;
using SubscriptionBillingAndNotificationCore.Infrastructure.Repository;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;

        public UserSubscriptionService(IUserSubscriptionRepository userSubscriptionRepository,IUserService userService, ISubscriptionService subscriptionService)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _userService = userService;
            _subscriptionService = subscriptionService;
        }

        public async Task<BaseResponse<string>> ActivateSubscription(ActivateSubscriptionRequestDto request)
        {
            var user = await _userService.GetUserById(request.UserId);
            var subscription = await _subscriptionService.GetSubscriptionById(request.SubsciptionPlanId);

            // check if user as an active subscription
            var userSubscription = _userSubscriptionRepository.SearchUserSubscriptions(userId : request.UserId).FirstOrDefault();
            if (userSubscription != null && userSubscription.SubscriptionStatus == SubscriptionStatus.Active)
                throw new Exception("Your already have an active subscription!");

            DateTime setStartDate = DateTime.UtcNow;
            DateTime setSubscriptionExpiry;

            switch (subscription.Data.Id)
            {
                case 1:
                    setSubscriptionExpiry = setStartDate.AddMonths(12);
                    break;
                case 2:
                    setSubscriptionExpiry = setStartDate.AddMonths(3);
                    break;
                case 3:
                    setSubscriptionExpiry = setStartDate.AddMonths(1);
                    break;
                default:
                    setSubscriptionExpiry = setStartDate;
                    break;
            }          
            
            if (userSubscription != null) 
            {
                userSubscription.SubscriptionPlanId = request.SubsciptionPlanId;
                userSubscription.StartDate = setStartDate;
                userSubscription.EndDate = setSubscriptionExpiry;
                userSubscription.SubscriptionStatus = SubscriptionStatus.Active;
                userSubscription.AutoRenew = request.AutoRenew;

                await _userSubscriptionRepository.UpdateUserSubscription(userSubscription);
                
            }
            else
            {
                userSubscription = new UserSubscription
                {
                    UserId = user.Data.Id,
                    SubscriptionPlanId = subscription.Data.Id,
                    StartDate = setStartDate,
                    EndDate = setSubscriptionExpiry,
                    NextBillingDate = setSubscriptionExpiry,
                    SubscriptionStatus = SubscriptionStatus.Active,
                    AutoRenew = request.AutoRenew
                };

                await _userSubscriptionRepository.AddUserSubscription(userSubscription);
            }

            // email user that subscription was successful

            return BaseResponse<string>.Ok("", "Subscription Activated Successfully");
        }

        public async Task<BaseResponse<string>> DeactivateSubscription(long userId)
        {
            var user = await _userService.GetUserById(userId);
            var userSubscription = _userSubscriptionRepository.SearchUserSubscriptions(userId, subscriptionStatus:  (int)SubscriptionStatus.Active).FirstOrDefault();
            if (userSubscription == null)
                throw new Exception("User does not have an active subscription!");

            userSubscription.SubscriptionStatus = SubscriptionStatus.Inactive;
            await _userSubscriptionRepository.UpdateUserSubscription(userSubscription);

            return BaseResponse<string>.Ok("", "Subscription Deactivated Successfully");
        }

    }
}
