using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult Signup(AuthRequestDto request)
        {
            try
            {
                var response = _authService.SignUp(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
          
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(AuthRequestDto request)
        {
            try
            {
                var response = _authService.Login(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
         
        }
    }
}
