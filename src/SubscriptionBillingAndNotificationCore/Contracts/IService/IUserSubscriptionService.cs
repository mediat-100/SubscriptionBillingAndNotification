using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;

namespace SubscriptionBillingAndNotificationCore.Contracts.IService
{
    public interface IUserSubscriptionService
    {
        Task<BaseResponse<string>> ActivateSubscription(ActivateSubscriptionRequestDto request, long userId, CancellationToken cancellationToken);
        Task<BaseResponse<string>> DeactivateSubscription(long userId, CancellationToken cancellationToken);
        Task<BaseResponse<UserSubscriptionResponseDto>> UserCurrentSubscription(long userId, CancellationToken cancellationToken);
        Task<BaseResponse<UserSubscriptionResponseDto>> UpgradeSubscription(long userId, long subscriptionId, CancellationToken ct);
        Task ProcessAdvanceReminders(CancellationToken cancellationToken);
        Task ProcessExpiryDayReminders(CancellationToken cancellationToken);
    }
}
