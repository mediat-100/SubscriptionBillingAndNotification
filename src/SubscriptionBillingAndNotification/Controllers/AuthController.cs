using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : BaseController
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
        [Route("Admin/Signup")]
        public async Task<IActionResult> AdminSignup(SignUpRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _authService.AdminSignup(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [Route("Admin/Login")]
        public async Task<IActionResult> AdminLogin(AuthRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _authService.AdminLogin(request, cancellationToken);
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
