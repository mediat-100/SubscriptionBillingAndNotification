using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<BaseResponse<string>> DeleteUser(long id)
        {
            var existingUser = await GetUserById(id);
            if (existingUser != null)
                throw new NotFoundException("User Id Not Found");

            var isDeleted = await _userRepository.DeleteUser(id);
            if (!isDeleted)
                throw new Exception("An error occurred while trying to delete user");

            return BaseResponse<string>.Ok("", "User Deleted Successfully");
        }

        public BaseResponse<IEnumerable<User>> GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            return BaseResponse<IEnumerable<User>>.Ok(users, "Users Fetched Successfully");
        }

        public async Task<BaseResponse<User>> GetUserById(long id)
        {
            var user = await _userRepository.GetUser(id);
            if (user == null)
                throw new NotFoundException("User Id Not Found");

            return BaseResponse<User>.Ok(user);
        }

        public BaseResponse<IEnumerable<User?>> SearchUsers(string email)
        {
            var users = _userRepository.SearchUsers(email);
          
            return BaseResponse<IEnumerable<User?>>.Ok(users);
        }
    }
}
