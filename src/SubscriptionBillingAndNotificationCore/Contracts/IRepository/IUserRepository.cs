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
        Task<User> AddUser(User user, CancellationToken cancellationToken);
        Task<User?> UpdateUser(User user, CancellationToken cancellationToken);
        Task<User?> GetUser(long userId, CancellationToken cancellationToken);
        Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);
        Task<bool> DeleteUser(long userId, CancellationToken cancellationToken);
        IEnumerable<User> SearchUsers(string? email, int? status = 1, int? userType = 2, int? isDeleted = 0, int pageNumber = 1, int pageSize = 10);
    }
}
