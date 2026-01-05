using Microsoft.EntityFrameworkCore;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddUser(User user, CancellationToken cancellationToken)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;      
        }

        public async Task<User?> GetUser(long userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Status == Enums.UserStatus.Active && !x.IsDeleted, cancellationToken);
        }

        public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email && x.Status == Enums.UserStatus.Active && !x.IsDeleted, cancellationToken);
        }

        public async Task<User?> UpdateUser(User user, CancellationToken cancellationToken)
        {
            var existingUser = await GetUser(user.Id, cancellationToken);
            if (existingUser == null)
                return null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user;
        }

        public async Task<bool> DeleteUser(long userId, CancellationToken cancellationToken)
        {
            var existingUser = await GetUser(userId, cancellationToken);
            if (existingUser == null)
                return false;

            existingUser.IsDeleted = true;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public IEnumerable<User> SearchUsers(string? email, int? status = 1, int? userType = 2, int? isDeleted = 0, int pageNumber = 1, int pageSize = 10)
        {

            IQueryable<User> query = _dbContext.Users;

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(x => x.Email == email);

            if (status > 0)
                query = query.Where(x => (int)x.Status == status);

            if (userType > 0)
                query = query.Where(x => (int)x.UserType == userType);

            if (isDeleted.HasValue && isDeleted == 1)
                query = query.Where(x => x.IsDeleted);

            var result = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

            return result;
          
        }

    }
}
