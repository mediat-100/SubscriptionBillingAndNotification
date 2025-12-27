using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
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
            var isDeleted = await _userRepository.DeleteUser(id);
            if (!isDeleted)
                throw new Exception("An error occurred while trying to delete user");

            return BaseResponse<string>.Ok("", "User Deleted Successfully");
        }

        public async Task<BaseResponse<UserResponseDto>> GetUserById(long id)
        {
            var user = await _userRepository.GetUser(id) ??
                throw new NotFoundException("User Id Not Found");

            var userResponseDto = ConvertToUserResponseDto(user);

            return BaseResponse<UserResponseDto>.Ok(userResponseDto, "User Fetched Successfully");
        }

        public BaseResponse<PagedUserResponseDto> SearchUsers(string? email, int? status = 1, int? userType = 2, int pageNumber = 1, int pageSize = 10)
        {
            var users = _userRepository.SearchUsers(email, status, userType, pageNumber: pageNumber, pageSize: pageSize);
            var result = users.Select(x => ConvertToUserResponseDto(x));
            var response = new PagedUserResponseDto { Users = result, PageNumber = pageNumber, PageSize = pageSize};

            return BaseResponse<PagedUserResponseDto>.Ok(response);
        }

        public async Task<BaseResponse<UserResponseDto>> UpdateUser(UpdateUserRequestDto request)
        {
            var user = await _userRepository.GetUser(request.Id) ??
                throw new NotFoundException("User Not Found!");

            user.Firstname = request.Firstname;
            user.Lastname = request.Lastname;
            user.RefreshToken = request.RefreshToken;
            user.RefreshTokenExpiryTime = request.RefreshTokenExpiryTime;

            var updatedUser = await _userRepository.UpdateUser(user);
            if (updatedUser == null)
                throw new Exception("User Update Failed, Please Try Again!");

            var userResponseDto = new UserResponseDto
            {
                Id = updatedUser.Id,
                Email = updatedUser.Email,
                Firstname = updatedUser.Firstname,
                Lastname = updatedUser.Lastname,
                Status = updatedUser.Status,
                UserType = updatedUser.UserType,
                RefreshToken = updatedUser.RefreshToken,
                RefreshTokenExpiryTime = updatedUser.RefreshTokenExpiryTime,
            };

            return BaseResponse<UserResponseDto>.Ok(userResponseDto, "User Updated Successfully!");
           
        }

        private UserResponseDto ConvertToUserResponseDto(User user)
        {
            var userResponseDto = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Status = user.Status,
                UserType = user.UserType,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
            };

            return userResponseDto;
        }

       
    }
}
