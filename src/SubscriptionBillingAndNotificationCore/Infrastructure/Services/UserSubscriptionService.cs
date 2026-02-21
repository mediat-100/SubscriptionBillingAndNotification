using System.Threading;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
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
        private readonly IWalletService _walletService;
        private readonly ILogger<UserSubscriptionService> _logger;

        public UserSubscriptionService(IUserSubscriptionRepository userSubscriptionRepository,IUserService userService, ISubscriptionService subscriptionService, 
            IEmailService emailService, IWalletService walletService, ILogger<UserSubscriptionService> logger)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _userService = userService;
            _subscriptionService = subscriptionService;
            _emailService = emailService;
            _walletService = walletService;
            _logger = logger;
        }

        public async Task<BaseResponse<string>> ActivateSubscription(ActivateSubscriptionRequestDto request, long userId, CancellationToken ct)
        {
            var user = await _userService.GetUserById(userId, ct);
            var subscription = await _subscriptionService.GetSubscriptionById(request.SubsciptionPlanId, ct);

            // check if user as an active subscription
            var userSubscription = _userSubscriptionRepository.SearchUserSubscriptions(userId : userId, subscriptionStatus: 1).OrderByDescending(x => x.EndDate).FirstOrDefault();
            if (userSubscription != null && userSubscription.EndDate >= DateTime.UtcNow)
                throw new ValidationException("You already have an active subscription!");

            // deduct subscription from user wallet
            var deductFunds = new DeductFundsRequestDto
            {
                Amount = subscription.Data.Pricing,
                Description = "SubscriptionPayment",
            };
            var transaction = await _walletService.DeductFunds(deductFunds, userId, ct);
            // check if charge was successful
            if (transaction.Data.Status != TransactionStatus.Completed.ToString())
                throw new Exception("Subscription Payment Failed!");

            DateTime setStartDate = DateTime.UtcNow;
            DateTime setSubscriptionExpiry;

            switch (subscription.Data.Id)
            {
                case 1:
                    setSubscriptionExpiry = setStartDate.AddMonths(1);
                    break;
                case 2:
                    setSubscriptionExpiry = setStartDate.AddMonths(3);
                    break;
                case 3:
                    setSubscriptionExpiry = setStartDate.AddMonths(12);
                    break;
                default:
                    setSubscriptionExpiry = setStartDate;
                    break;
            }

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

            await _userSubscriptionRepository.AddUserSubscription(userSubscription, ct);

            await _emailService.SendEmail(userSubscription.User.Email, "Subscription Activated!", "Your subscription is now active");

            return BaseResponse<string>.Ok("", "Subscription Activated Successfully");
           
        }

        public async Task<BaseResponse<string>> DeactivateSubscription(long userId, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserById(userId, cancellationToken);
            var userSubscription = _userSubscriptionRepository.SearchUserSubscriptions(userId, subscriptionStatus:  (int)SubscriptionStatus.Active).OrderByDescending(x => x.StartDate).FirstOrDefault();
            if (userSubscription == null || userSubscription.EndDate <= DateTime.UtcNow)
                throw new ValidationException("User does not have an active subscription!");

            userSubscription.AutoRenew = false;
            userSubscription.CancelAtExpiry = true;
            userSubscription.CancellationReason = SubCancellationReason.UserInitiated;
            userSubscription.CancelledAt = DateTime.UtcNow;

            await _userSubscriptionRepository.UpdateUserSubscription(userSubscription, cancellationToken);

            await _emailService.SendEmail(userSubscription.User.Email, "Subscription Deactivated!", "Your subscription has now been deactived");

            return BaseResponse<string>.Ok("", "Subscription Deactivated Successfully");
        }

        public async Task<BaseResponse<UserSubscriptionResponseDto>> UserCurrentSubscription(long userId, CancellationToken ct)
        {
            // check if user exist
            var user = await _userService.GetUserById(userId, ct);
            var current_user_subscription = _userSubscriptionRepository.SearchUserSubscriptions(userId: userId, subscriptionStatus: (int)SubscriptionStatus.Active).OrderByDescending(x => x.StartDate).FirstOrDefault();
            
            // 2fa
            if (current_user_subscription is null || current_user_subscription.EndDate <= DateTime.UtcNow)
                throw new NotFoundException("You currently don't have an active subscription");

            var currentUser_SubscriptionResponse = new UserSubscriptionResponseDto()
            {
                UserId = current_user_subscription.UserId,
                SubscriptionPlanId = current_user_subscription.SubscriptionPlanId,
                SubscriptionStatus = current_user_subscription.SubscriptionStatus.ToString(),
                SubcriptionStartDateTime = current_user_subscription.StartDate,
                SubscriptionExpiryDateTime = current_user_subscription.EndDate,
                AutoRenew = current_user_subscription.AutoRenew,
            };

            return BaseResponse<UserSubscriptionResponseDto>.Ok(currentUser_SubscriptionResponse);
        }

        public async Task<BaseResponse<UserSubscriptionResponseDto>> UpgradeSubscription(long userId, long subPlanId, CancellationToken ct)
        {
            var user = await _userService.GetUserById(userId, ct);
            var current_user_subscription = _userSubscriptionRepository.SearchUserSubscriptions(userId: userId, subscriptionStatus: (int)SubscriptionStatus.Active).OrderByDescending(x => x.StartDate).FirstOrDefault();

            // 2fa
            if (current_user_subscription is null || current_user_subscription.EndDate <= DateTime.UtcNow)
                throw new NotFoundException("You currently don't have an active subscription");

            // sub end date won't chage user just add more money
            // compare the prices only allow upgrade
            var currentSubPlan = await _subscriptionService.GetSubscriptionById(current_user_subscription.SubscriptionPlanId, ct);
            var newSubscription = await _subscriptionService.GetSubscriptionById(subPlanId, ct);
            if (currentSubPlan.Data.Id == newSubscription.Data.Id)
                throw new ValidationException("You are already subscribed to this plan");

            if (newSubscription.Data.Pricing < currentSubPlan.Data.Pricing)
                throw new ValidationException("Invalid upgrade!");

            decimal subDifference = newSubscription.Data.Pricing - currentSubPlan.Data.Pricing;
            // charge subdifference
            var chargeRequest = new DeductFundsRequestDto
            {
                Amount = subDifference,
                Description = "Upgrade Subscription"
            };
            var transaction = await _walletService.DeductFunds(chargeRequest, userId, ct);
            // check if charge was successful
            if (transaction.Data.Status != TransactionStatus.Completed.ToString())
                throw new Exception("Subscription Payment Failed!");

            // update plan
            current_user_subscription.SubscriptionPlanId = newSubscription.Data.Id;

            current_user_subscription.EndDate = newSubscription.Data.Id switch
            {
                2 => current_user_subscription.StartDate.AddMonths(3),
                3 => current_user_subscription.StartDate.AddMonths(12),
                _ => current_user_subscription.EndDate
            };

            var update_user_sub = await _userSubscriptionRepository.UpdateUserSubscription(current_user_subscription, ct);
            await _emailService.SendEmail(user.Data.Email, "Subscription Upgraded!", "Your subscription has been upgraded");
            var response = new UserSubscriptionResponseDto
            {
                UserId = update_user_sub.UserId,
                SubscriptionPlanId = update_user_sub.SubscriptionPlanId,
                SubcriptionStartDateTime = update_user_sub.StartDate,
                SubscriptionExpiryDateTime = update_user_sub.EndDate,
                SubscriptionStatus = update_user_sub.SubscriptionStatus.ToString(),
                AutoRenew = update_user_sub.AutoRenew
            };

            return BaseResponse<UserSubscriptionResponseDto>.Ok(response);

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
                    await _emailService.SendEmail(userSubscription.User.Email, "Subscription Expiring Soon!!!", "Your subscription is about to expire. Please renew!");

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
                    await _emailService.SendEmail(userSubscription.User.Email, "Your subscription has expired!!!", "Your subscription has expired. Please renew!");

                    userSubscription.ExpiryDayReminderSent = true;
                    await _userSubscriptionRepository.UpdateUserSubscription(userSubscription, cancellationToken);

                    await Task.Delay(1000, cancellationToken);
                }
                   
            }
           
        }
    }
}
