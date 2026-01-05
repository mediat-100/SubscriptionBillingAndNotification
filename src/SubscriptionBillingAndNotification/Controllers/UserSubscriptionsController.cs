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
        public async Task<IActionResult> Activate(ActivateSubscriptionRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _userSubscriptionService.ActivateSubscription(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        [Route("Deactivate")]
        public async Task<IActionResult> Deactivate(long userId, CancellationToken cancellationToken)
        {
            var response = await _userSubscriptionService.DeactivateSubscription(userId, cancellationToken);
            return Ok(response);
        }
    }
}
