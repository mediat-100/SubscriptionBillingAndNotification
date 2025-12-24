using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Utilities.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Service
{
    public class TokenService : ITokenService
    {
        //private readonly ITokenGenerator _tokenGenerator;
        private readonly string? _secretKey;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public TokenService(IConfiguration configuration, IUserRepository userRepository, IUserService userService)
        {
            // _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
            _userService = userService;
            _configuration = configuration;
            _userRepository = userRepository;
            _secretKey = _configuration.GetSection("Jwt")["Key"];
            _issuer = _configuration.GetSection("Jwt")["Issuer"];
            _audience = _configuration.GetSection("Jwt")["Audience"];


        }

        public async Task<AuthResponseDto> AuthenticateUser(User user)
        {
            // generate token and refresh token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                //new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var generateAccessToken = GenerateAccessToken(claims);
            string refreshToken = GenerateRefreshToken();

            // update refresh token expiry time
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(15);
            await _userRepository.UpdateUser(user);

            var response = new AuthResponseDto
            {
                Email = user.Email,
                UserId = user.Id,
                AccessToken = generateAccessToken.AccessToken,
                AccessTokenExpiresAt = generateAccessToken.AccessTokenExpiresAt,
                RefreshToken = refreshToken
            };

            return response;
        }

        public async Task<RefreshTokenResponseDto> RefreshToken(RefreshTokenRequestDto request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.Identity.Name;
            var user = await _userService.GetUserById(int.Parse(userId));
            if (!(user.Data.Id >= 0))
                throw new Exception("Invalid user!");

            if (user.Data.RefreshToken != null && user.Data.RefreshTokenExpiryTime < DateTime.UtcNow)
                throw new Exception("Invalid Refresh Token!");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Data.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Data.UserType.ToString())
            };

            var accessToken = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken();

            var updateUserRequest = new UpdateUserRequestDto
            {
                Id = user.Data.Id,
                Firstname = user.Data.Firstname,
                Lastname = user.Data.Lastname,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(15),
            };

            var updatedUser = await _userService.UpdateUser(updateUserRequest);
 
            return new RefreshTokenResponseDto { AccessToken = accessToken.AccessToken, AccessTokenExpiresAt = accessToken.AccessTokenExpiresAt, RefreshToken = refreshToken };
        }

        private AccessTokenResponse GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AccessTokenResponse { AccessToken  = accessToken, AccessTokenExpiresAt = token.ValidTo };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                ValidateLifetime = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
