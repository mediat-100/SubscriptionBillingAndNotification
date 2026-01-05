using Microsoft.Extensions.Logging;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Enums;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;
using SubscriptionBillingAndNotificationCore.Infrastructure.Repository;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserSubscriptionService> _logger;

        public UserSubscriptionService(IUserSubscriptionRepository userSubscriptionRepository,IUserService userService, ISubscriptionService subscriptionService, 
            IEmailService emailService, ILogger<UserSubscriptionService> logger)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _userService = userService;
            _subscriptionService = subscriptionService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<BaseResponse<string>> ActivateSubscription(ActivateSubscriptionRequestDto request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserById(request.UserId, cancellationToken);
            var subscription = await _subscriptionService.GetSubscriptionById(request.SubsciptionPlanId, cancellationToken);

            // check if user as an active subscription
            var userSubscription = _userSubscriptionRepository.SearchUserSubscriptions(userId : request.UserId).FirstOrDefault();
            if (userSubscription != null && userSubscription.SubscriptionStatus == SubscriptionStatus.Active)
                throw new ValidationException("Your already have an active subscription!");

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

                await _userSubscriptionRepository.UpdateUserSubscription(userSubscription, cancellationToken);
                
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

                await _userSubscriptionRepository.AddUserSubscription(userSubscription, cancellationToken);
            }

            // email user that subscription was successful
            _emailService.SendEmail(userSubscription.User.Email, "Subscription Activated!", "Your subscription is now active");
            return BaseResponse<string>.Ok("", "Subscription Activated Successfully");
        }

        public async Task<BaseResponse<string>> DeactivateSubscription(long userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserById(userId, cancellationToken);
            var userSubscription = _userSubscriptionRepository.SearchUserSubscriptions(userId, subscriptionStatus:  (int)SubscriptionStatus.Active).FirstOrDefault();
            if (userSubscription == null)
                throw new ValidationException("User does not have an active subscription!");

            userSubscription.SubscriptionStatus = SubscriptionStatus.Inactive;
            await _userSubscriptionRepository.UpdateUserSubscription(userSubscription, cancellationToken);

            _emailService.SendEmail(userSubscription.User.Email, "Subscription Deactivated!", "Your subscription has now been deactived");
            return BaseResponse<string>.Ok("", "Subscription Deactivated Successfully");
        }

        public async Task ProcessAdvanceReminders(CancellationToken cancellationToken)
        {
            // check subscriptions about to expire in 3 days and send a reminder via email;
            var subscriptionsExpiresIn3days = await _userSubscriptionRepository.GetSubscriptionsExpiringIn3days(cancellationToken);
            if (subscriptionsExpiresIn3days.Count > 0)
            {
                foreach (var userSubscription in subscriptionsExpiresIn3days)
                {
                    // send a mail reminder
                    _emailService.SendEmail(userSubscription.User.Email, "Subscription Expiring Soon!!!", "Your subscription is about to expire. Please renew!");

                    // update db that mail has been sent
                    userSubscription.AdvanceReminderSent = true;
                    await _userSubscriptionRepository.UpdateUserSubscription(userSubscription, cancellationToken);

                    await Task.Delay(1000, cancellationToken);
                }
            }
            
        }

        public async Task ProcessExpiryDayReminders(CancellationToken cancellationToken)
        {
            var subscriptionsExpiresIn3days = await _userSubscriptionRepository.GetExpiredSubscriptions(cancellationToken);
            if (subscriptionsExpiresIn3days.Count > 0)
            {
                foreach (var userSubscription in subscriptionsExpiresIn3days)
                {
                     // send a mail reminder
                    _emailService.SendEmail(userSubscription.User.Email, "Your subscription has expired!!!", "Your subscription has expired. Please renew!");

                    userSubscription.ExpiryDayReminderSent = true;
                    await _userSubscriptionRepository.UpdateUserSubscription(userSubscription, cancellationToken);

                    await Task.Delay(1000, cancellationToken);
                }
                   
            }
           
        }
    }
}
