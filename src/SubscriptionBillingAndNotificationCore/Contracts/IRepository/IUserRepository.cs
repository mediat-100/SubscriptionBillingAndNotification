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
        Task<User?> UpdateUser(User user);
        Task<User?> GetUser(long userId);
        IEnumerable<User> GetAllUsers(int pageNumber = 1, int pageSize = 10);
        Task<bool> DeleteUser(long userId);
        IEnumerable<User> SearchUsers(string? email, int status = 2, int userType = 2, int pageNumber = 1, int pageSize = 10);
    }
}
