using Microsoft.Extensions.Configuration;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly string? secretkey;
        private readonly string? issuer;
        private readonly string? audience;

        public AuthService(ITokenService tokenService, IUserRepository userRepository, IUserService userService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _userService = userService;
            _configuration = configuration;
            secretkey = _configuration.GetSection("Jwt")["secretkey"];
            issuer = _configuration.GetSection("Jwt")["issuer"];
            audience = _configuration.GetSection("Jwt")["audience"];
        }

        public async Task<BaseResponse<AuthResponseDto>> SignUp(AuthRequestDto request)
        {
            try
            {
                // check if user already exist
                var existingUser = _userService.SearchUsers(email: request.Email);
                if (existingUser.Count > 0)
                    throw new Exception("User already exist");

                // save user to db
                var user = new User
                {
                    Email = request.Email,
                    Password = Helpers.HashPassword(request.Password),
                };

                await _userRepository.AddUser(user);

                // generate token and refresh token
               var authResponse = await _tokenService.AuthenticateUser(user);

                return BaseResponse<AuthResponseDto>.Ok(authResponse, "Signup successful");
            }
            catch (Exception ex)
            {
                return BaseResponse<AuthResponseDto>.Fail(ex.Message);
            }
           
        }

        public async Task<BaseResponse<AuthResponseDto>> Login(AuthRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    throw new Exception("Please input your email and password");
                // check if email exist
                var user = _userRepository.SearchUsers(email: request.Email).FirstOrDefault() 
                    ?? throw new Exception("User does not exist!");
                // check if password is correct
                var hashedPassword = Helpers.HashPassword(request.Password);
                var verifyPassword = Helpers.VerifyPassword(request.Password, hashedPassword);
                if (!verifyPassword)
                    throw new Exception("Incorrect Email or Password");
                // login user
                var authResponse = await _tokenService.AuthenticateUser(user);

                return BaseResponse<AuthResponseDto>.Ok(authResponse, "Login successful");
            }
            catch(Exception ex)
            {
                return BaseResponse<AuthResponseDto>.Fail(ex.Message);
            }
        }


        
    }
}
