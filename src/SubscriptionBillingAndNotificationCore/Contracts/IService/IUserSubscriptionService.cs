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
        Task<BaseResponse<string>> ActivateSubscription(ActivateSubscriptionRequestDto request);
        Task<BaseResponse<string>> DeactivateSubscription(long userId);
    }
}
