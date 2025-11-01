using SubscriptionBillingAndNotificationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Contracts.IRepository
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<User?> GetUser(long userId);
        IEnumerable<User> GetAllUsers();
        Task<bool> DeleteUser(long userId);
        List<User> SearchUsers(string email);
    }
}
