using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionBillingAndNotificationCore.Contracts.IService
{
    public interface IAuthService
    {
        Task<BaseResponse<AuthResponseDto>> SignUp(SignUpRequestDto request, CancellationToken cancellationToken);
        Task<BaseResponse<AuthResponseDto>> Login(AuthRequestDto request, CancellationToken cancellationToken);
        Task<BaseResponse<RefreshTokenResponseDto>> RefreshToken(RefreshTokenRequestDto request, CancellationToken cancellationToken); 
    }
}
