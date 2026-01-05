using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Contracts.IService
{
    public interface ITokenService
    {
        Task<AuthResponseDto> AuthenticateUser(User user, CancellationToken cancellationToken);
        Task<RefreshTokenResponseDto> RefreshToken(RefreshTokenRequestDto request, CancellationToken cancellationToken);
    }
}
