using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Contracts.IService
{
    public interface ISubscriptionService
    {
        Task<BaseResponse<SubscriptionResponseDto>> AddSubscription(AddSubscriptionRequestDto subscription, CancellationToken cancellationToken);
        BaseResponse<IEnumerable<SubscriptionResponseDto>> SearchSubscriptions(string type, int pageNumber = 1, int pageSize = 10);
        Task<BaseResponse<SubscriptionResponseDto>> GetSubscriptionById(long id, CancellationToken cancellationToken);
        Task<BaseResponse<SubscriptionResponseDto>> UpdateSubscription(UpdateSubscriptionRequestDto subscription, CancellationToken cancellationToken);
        Task<BaseResponse<string>> DeleteSubscription(long id, CancellationToken cancellationToken);
    }
}
