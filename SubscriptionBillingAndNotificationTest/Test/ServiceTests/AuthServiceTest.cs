using Microsoft.EntityFrameworkCore;
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

namespace Test.ServiceTests
{
    public class AuthServiceTest
    {
        private readonly IFixture _fixture;
        private readonly IAuthService _authService;
        //private readonly ITokenGenerator _tokenGenerator;

        //private readonly ITokenService _tokenService;
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
           // _tokenService = new TokenService(_tokenGeneratorMock.Object, _userRepositoryMock.Object);
           // _tokenGenerator = new TokenGenerator(config, _userRepositoryMock.Object);
            _authService = new AuthService(_tokenServiceMock.Object, _userRepositoryMock.Object, _userServiceMock.Object, config);
        }

        #region SignUp

        [Fact]
        public async Task SignUp_WithNewUser_ShouldReturnSuccessResponse()
        {

                var authRequest = _fixture.Build<AuthRequestDto>()
            .With(x => x.Email, "abc@email.com")
            .With(x => x.Password, "12345678") 
            .Create();

                var authResponse = _fixture.Build<AuthResponseDto>()
                    .With(x => x.Email, "abc@email.com")
                    .With(x => x.AccessToken, "sample-token")
                    .With(x => x.RefreshToken, "sample-refresh-token")
                    .Create();

                // Mock: User doesn't exist yet
                _userServiceMock.Setup(x => x.SearchUsers(authRequest.Email))
                    .Returns(new List<User>());

                // Mock: AddUser completes successfully
                _userRepositoryMock.Setup(x => x.AddUser(It.IsAny<User>()))
                    .ReturnsAsync(It.IsAny<User>);  

                // Mock: Token generation returns our auth response
                _tokenServiceMock.Setup(x => x.AuthenticateUser(It.IsAny<User>()))
                    .ReturnsAsync(authResponse);

                // Act
                var result = await _authService.SignUp(authRequest);

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue();
                result.Message.Should().Be("Signup successful");
                result.Data.Should().NotBeNull();
                result.Data.Email.Should().Be("abc@email.com");
                result.Data.AccessToken.Should().Be("sample-token");
                result.Data.RefreshToken.Should().Be("sample-refresh-token");

                _userRepositoryMock.Verify(x => x.AddUser(It.Is<User>(u =>
            u.Email == authRequest.Email &&
            u.Password != authRequest.Password  // Password should be hashed
        )), Times.Once);
            }

        [Fact]
        public async Task SignUp_WithExistingUser_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "existing@example.com",
                Password = "Password123!"
            };

            var existingUsers = new List<User>
            {
                new User { Email = request.Email, Password = "hashedpassword" }
            };

            _userServiceMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(existingUsers);

            // Act
            var result = await _authService.SignUp(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User already exist", result.Message);
            Assert.Null(result.Data);
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
                Password = "Password123!"
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

            _userRepositoryMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(new List<User> { user });

         //   _tokenServiceMock.Setup(x => x.AuthenticateUser(It.IsAny<User>())).ReturnsAsync(authResponse);

            // Act
            var result = await _authService.Login(request);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Login successful", result.Message);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task Login_WithNonExistentEmail_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(new List<User>());

            // Act
            var result = await _authService.Login(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User does not exist!", result.Message);
            Assert.Null(result.Data);
        }

       /* [Fact]
        public async Task Login_WithIncorrectPassword_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "user@example.com",
                Password = "WrongPassword"
            };

            var correctHashedPassword = Helpers.HashPassword("CorrectPassword123");
            var user = new User
            {
                Email = request.Email,
                Password = correctHashedPassword
            };

            _userRepositoryMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(new List<User> { user });


            // Act
            var result = await _authService.Login(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Incorrect Email or Password", result.Message);
            Assert.Null(result.Data);
        }*/

        [Fact]
        public async Task Login_WhenRepositoryThrowsException_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "user@example.com",
                Password = "Password123!"
            };

            _userRepositoryMock.Setup(x => x.SearchUsers(request.Email))
                .Throws(new Exception("An error occurred, Please try again!"));

            // Act
            var result = await _authService.Login(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("An error occurred, Please try again!", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task Login_WithEmptyPassword_ShouldReturnFailureResponse()
        {
            // Arrange
            var request = new AuthRequestDto
            {
                Email = "user@example.com",
                Password = ""
            };

            var user = new User
            {
                Email = request.Email,
                Password = Helpers.HashPassword("ActualPassword")
            };

            _userRepositoryMock.Setup(x => x.SearchUsers(request.Email))
                .Returns(new List<User> { user });

            _tokenServiceMock.Setup(x => x.AuthenticateUser(It.IsAny<User>())).ReturnsAsync(It.IsAny<AuthResponseDto>());

            // Act
            var result = await _authService.Login(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Please input your email and password", result.Message);
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

    public class HelpersMock
    {
        public bool  SetupVerifyPassword(bool value)
        {
            return false;
            // No-op placeholder to represent a successful password verification.
            // In real-world tests, just let Helpers.VerifyPassword() run directly.
        }
    }
}


