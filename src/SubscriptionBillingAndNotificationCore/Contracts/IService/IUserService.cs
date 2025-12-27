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
    public interface IUserService
    {
        BaseResponse<PagedUserResponseDto> SearchUsers(string? email, int? status = 1, int? userType = 2, int pageNumber = 1, int pageSize = 10);
        Task<BaseResponse<UserResponseDto>> GetUserById(long id);
        Task<BaseResponse<string>> DeleteUser(long id);
        Task<BaseResponse<UserResponseDto>> UpdateUser(UpdateUserRequestDto request);
    }
}
