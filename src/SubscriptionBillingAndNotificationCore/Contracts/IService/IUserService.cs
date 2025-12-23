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
        BaseResponse<IEnumerable<User>> SearchUsers(string email);
        BaseResponse<IEnumerable<User>> GetAllUsers();
        Task<BaseResponse<User>> GetUserById(long id);
        Task<BaseResponse<string>> DeleteUser(long id);
    }
}
