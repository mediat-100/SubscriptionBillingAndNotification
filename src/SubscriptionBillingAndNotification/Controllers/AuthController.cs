using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("/signup")]
        public IActionResult Signup(SignUpRequestDto request)
        {
            var response = _authService.SignUp(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(AuthRequestDto request)
        {
            var response = _authService.Login(request);
            return Ok(response);
        }
    }
}
