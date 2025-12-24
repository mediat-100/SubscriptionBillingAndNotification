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
        public async Task<IActionResult> Activate(ActivateSubscriptionRequestDto request)
        {
            var response = await _userSubscriptionService.ActivateSubscription(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("Deactivate")]
        public async Task<IActionResult> Deactivate(long userId)
        {
            var response = await _userSubscriptionService.DeactivateSubscription(userId);
            return Ok(response);
        }
    }
}
