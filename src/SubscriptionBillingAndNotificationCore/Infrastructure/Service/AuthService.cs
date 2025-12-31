using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Enums;
using SubscriptionBillingAndNotificationCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Http.ModelBinding;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly string? secretkey;
        private readonly string? issuer;
        private readonly string? audience;

        public AuthService(ITokenService tokenService, IUserRepository userRepository, IUserService userService, IConfiguration configuration
            , IEmailService emailService)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _userService = userService;
            _configuration = configuration;
            _emailService = emailService;
            secretkey = _configuration.GetSection("Jwt")["secretkey"];
            issuer = _configuration.GetSection("Jwt")["issuer"];
            audience = _configuration.GetSection("Jwt")["audience"];
        }

        public async Task<BaseResponse<AuthResponseDto>> SignUp(SignUpRequestDto request)
        {
            if (!Helpers.IsValidEmail(request.Email))
                throw new ValidationException("A valid email is required.");

            if (request.Password.Length < 8)
                throw new ValidationException("Password must be at least 8 characters long.");

            if (!request.Password.Any(char.IsUpper))
                throw new ValidationException("Password must contain at least one uppercase letter.");

            if (!request.Password.Any(char.IsLower))
                throw new ValidationException("Password must contain at least one lowercase letter.");

            if (!request.Password.Any(char.IsDigit))
                throw new ValidationException("Password must contain at least one number.");

            if (!request.Password.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>?/".Contains(ch)))
                throw new ValidationException("Password must contain at least one special character.");

            if (request.Password.Contains(" "))
                throw new ValidationException("Password cannot contain spaces.");

            var existingUser = _userService.SearchUsers(email: request.Email);
            if (existingUser.Data.Users.Count() > 0)
                throw new ValidationException("User already exist!");

            var user = new User
            {
                Firstname = request.Firstname,
                Lastname = request.Lastname,
                Email = request.Email,
                Password = Helpers.HashPassword(request.Password),
                Status = UserStatus.Active,
                UserType = UserType.User
            };

            var userResp = await _userRepository.AddUser(user);

            //TODO: 1) SEND CONFIRMATION MAIL TO USER AND ACTIVATE USER AFTER EMAIL CONFIRMATION, 2) AUTHENTICATE VIA OAUTH
            var authResponse = await _tokenService.AuthenticateUser(user);

            // send
            _emailService.SendEmail("mediatyusuff@gmail.com", "Registration Successful", "Your sign up was successful");

            return BaseResponse<AuthResponseDto>.Ok(authResponse, "Signup successful");
           
        }

        public async Task<BaseResponse<AuthResponseDto>> Login(AuthRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                throw new ValidationException("Please input your email and password");

            var user = await _userRepository.GetUserByEmail(request.Email) ??
                throw new UnauthorizedException("Incorrect Email or Password");
            
            var verifyPassword = Helpers.VerifyPassword(request.Password, user.Password);
            if (!verifyPassword)
                throw new UnauthorizedException("Incorrect Email or Password!");

            var authResponse = await _tokenService.AuthenticateUser(user);

            return BaseResponse<AuthResponseDto>.Ok(authResponse, "Login successful");
            
        }

        public async Task<BaseResponse<RefreshTokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var response = await _tokenService.RefreshToken(request);
            return BaseResponse<RefreshTokenResponseDto>.Ok(response);
        }

        
    }
}
