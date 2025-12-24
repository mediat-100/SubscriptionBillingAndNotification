/*using Microsoft.EntityFrameworkCore;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using AutoFixture;
using Moq;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Infrastructure.Service;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using FluentAssertions;
using SubscriptionBillingAndNotification.Middlewares;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;
using Microsoft.Extensions.Configuration;
using SubscriptionBillingAndNotificationCore.Utilities;
using System.Security.Claims;
using Azure.Core;
using Azure;
using Microsoft.IdentityModel.Tokens;

namespace Test.ServiceTests
{
    public class AuthServiceTest
    {
        private readonly IFixture _fixture;
        private readonly IAuthService _authService;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITokenGenerator> _tokenGeneratorMock;

        public AuthServiceTest()
        {
            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            ApplicationDbContext dbContext = dbContextMock.Object;


            _fixture = new Fixture();
            _userServiceMock = new Mock<IUserService>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _tokenGeneratorMock = new Mock<ITokenGenerator>();

            var config = CreateConfiguration();
            _authService = new AuthService(_tokenServiceMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, config);
        }

        #region SignUp

        [Fact]
        public async Task SignUp_WithNewUser_ShouldReturnSuccessResponse()
        {
            // Arrange
           var signupRequest = _fixture.Build<SignUpRequestDto>()
            .With(x => x.Email, "abc@email.com")
            .With(x => x.Password, "Abc1234567&") 
            .Create();

                var authResponse = _fixture.Build<AuthResponseDto>()
                    .With(x => x.Email, "abc@email.com")
                    .With(x => x.AccessToken, "sample-token")
                    .With(x => x.RefreshToken, "sample-refresh-token")
                    .Create();

            _userServiceMock.Setup(x => x.SearchUsers(signupRequest.Email))
                    .Returns(BaseResponse<IEnumerable<User>>.Ok(new List<User>()));

            _userRepositoryMock.Setup(x => x.AddUser(It.IsAny<User>()))
                .ReturnsAsync(It.IsAny<User>);  

            _tokenServiceMock.Setup(x => x.AuthenticateUser(It.IsAny<User>()))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authService.SignUp(signupRequest);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Signup successful");
            result.Data.Should().NotBeNull();
            result.Data.Email.Should().Be("abc@email.com");
            result.Data.AccessToken.Should().Be("sample-token");
            result.Data.RefreshToken.Should().Be("sample-refresh-token");

            *//*_userRepositoryMock.Verify(x => x.AddUser(It.Is<User>(u =>
        u.Email == signupRequest.Email &&
        u.Password != signupRequest.Password  // Password should be hashed
    )), Times.Once);*//*
            }

        [Fact]
        public async Task SignUp_WithExistingUser_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new SignUpRequestDto
            {
                Email = "existing@example.com",
                Password = "Password123!"
            };

            var existingUsers = new List<User>
            {
                new User { Email = request.Email, Password = "hashedpassword" }
            };

            _userServiceMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(BaseResponse<IEnumerable<User>>.Ok(existingUsers));

            Func<Task> action = async () =>
            {
                // act
                await _authService.SignUp(request);
            };

            // Assert
            await action.Should().ThrowAsync<ValidationException>();
        }


        #endregion

        #region Login

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnSuccessResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "user@example.com",
                Password = "Abc1234567&"
            };

            var user = new User
            {
                Email = request.Email,
                Password = Helpers.HashPassword(request.Password)
            };

            var authResponse = new AuthResponseDto
            {
                Email = user.Email,
                UserId = user.Id,
                AccessToken = "accessToken",
                RefreshToken = "refreshToken",
            };

            _userServiceMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(BaseResponse<IEnumerable<User>>.Ok(new List<User> { user }));

            _tokenServiceMock.Setup(x => x.AuthenticateUser(It.IsAny<User>())).ReturnsAsync(authResponse);

            // Act
            var result = await _authService.Login(request);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Login successful");
            result.Data.Should().NotBeNull();
        }


        [Fact]
        public async Task Login_UserNotFound_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _userServiceMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(BaseResponse<IEnumerable<User>>.Ok(new List<User> { }));

            // Act
            Func<Task> action = async () =>
            {
                // act
                await _authService.Login(request);
            };

            // Assert
            await action.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task Login_InvalidCredentials_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Email = "nonexistent@example.com",
                Password = Helpers.HashPassword("12345678")
            };

            _userServiceMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(BaseResponse<IEnumerable<User>>.Ok(new List<User> { }));

            Helpers.VerifyPassword(request.Password, user.Password);

            // Act
            Func<Task> action = async () =>
            {
                // act
                await _authService.Login(request);
            };

            // Assert
            await action.Should().ThrowAsync<UnauthorizedException>();
        }

        #endregion

        private IConfiguration CreateConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:secretkey", "test-secret-key-123"},
            {"Jwt:issuer", "test-issuer"},
            {"Jwt:audience", "test-audience"}
        };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }
    }

}


*/