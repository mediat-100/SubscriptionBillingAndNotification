using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> Signup(SignUpRequestDto request, CancellationToken cancellationToken)
        {                
            var response = await _authService.SignUp(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(AuthRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _authService.Login(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _authService.RefreshToken(request, cancellationToken);
            return Ok(response);
        }
    }
}
