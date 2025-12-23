using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserSubscriptionsController : Controller
    {
        private readonly IUserSubscriptionService _userSubscriptionService;

        public UserSubscriptionsController(IUserSubscriptionService userSubscriptionService)
        {
            _userSubscriptionService = userSubscriptionService;
        }


        [HttpPost]
        [Route("Activate")]
        public IActionResult Activate(ActivateSubscriptionRequestDto request)
        {
            var response = _userSubscriptionService.ActivateSubscription(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("Deactivate")]
        public IActionResult Deactivate(long userId)
        {
            var response = _userSubscriptionService.DeactivateSubscription(userId);
            return Ok(response);
        }
    }
}
