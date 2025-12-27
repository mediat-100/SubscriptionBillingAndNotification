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
        Task<User?> GetUserByEmail(string email);
        Task<bool> DeleteUser(long userId);
        IEnumerable<User> SearchUsers(string? email, int? status = 1, int? userType = 2, int? isDeleted = 0, int pageNumber = 1, int pageSize = 10);
    }
}
