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

        public async Task<User> AddUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;      
        }

        public IEnumerable<User> GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            return _dbContext.Users.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();
        }

        public async Task<User?> GetUser(long userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && x.Status == Enums.UserStatus.Active && !x.IsDeleted);
        }

        public async Task<User?> UpdateUser(User user)
        {
            var existingUser = await GetUser(user.Id);
            if (existingUser == null)
                return null;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DeleteUser(long userId)
        {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
                return false;

            existingUser.IsDeleted = true;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public IEnumerable<User> SearchUsers(string? email, int status = 2, int userType = 2, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<User> query = _dbContext.Users;

                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(x => x.Email == email);

                if (status > 0)
                    query = query.Where(x => (int)x.Status == status);

                if (userType > 0)
                    query = query.Where(x => (int)x.UserType == userType);

                var result = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsEnumerable();

                return result;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred");
            }
          
        }

    }
}
