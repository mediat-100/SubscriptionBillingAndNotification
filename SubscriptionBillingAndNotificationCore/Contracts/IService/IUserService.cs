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
        List<User> SearchUsers(string email);
        IEnumerable<User> GetAllUsers();
        Task<User> GetUserById(long id);
        Task<string> DeleteUser(long id);
    }
}
