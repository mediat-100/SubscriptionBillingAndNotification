using Microsoft.EntityFrameworkCore;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            try
            {
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred");
            }

           
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _dbContext.Users.ToList().AsEnumerable();
        }

        public async Task<User?> GetUser(long userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsDeleted == false);
        }

        public async Task<User> UpdateUser(User user)
        {
            var existingUser = await GetUser(user.Id);
            if (existingUser == null)
                return user;

            _dbContext.Users.Update(existingUser);
            await _dbContext.SaveChangesAsync();

            return existingUser;
        }

        public async Task<bool> DeleteUser(long userId)
        {
            var existingUser = await GetUser(userId);
            if (existingUser == null)
                return false;

            _dbContext.Users.Remove(existingUser);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public List<User> SearchUsers(string email)
        {
            try
            {
                var result = _dbContext.Users.Where(x => x.Email == email).ToList();
                return result;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occurred");
            }
          
        }

    }
}
