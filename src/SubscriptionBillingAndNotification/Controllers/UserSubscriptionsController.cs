using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Infrastructure.Services;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [Authorize]
    public class UserSubscriptionsController : BaseController
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private long UserId => long.Parse(GetCurrentUserId());

        public UserSubscriptionsController(IUserSubscriptionService userSubscriptionService)
        {
            _userSubscriptionService = userSubscriptionService;
        }


        [HttpPost]
        [Route("Activate")]
        public async Task<IActionResult> Activate(ActivateSubscriptionRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _userSubscriptionService.ActivateSubscription(request, UserId, cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        [Route("Deactivate")]
        public async Task<IActionResult> Deactivate(CancellationToken cancellationToken)
        {
            var response = await _userSubscriptionService.DeactivateSubscription(UserId, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route("Current")]
        public async Task<IActionResult> CurrentSubscription(CancellationToken ct)
        {
            var response = await _userSubscriptionService.UserCurrentSubscription(UserId, ct);
            return Ok(response);
        }

        [HttpPost]
        [Route("Upgrade")]
        public async Task<IActionResult> Upgrade(long subscriptionPlanId,CancellationToken ct)
        {
            var response = await _userSubscriptionService.UpgradeSubscription(UserId, subscriptionPlanId, ct);
            return Ok(response);
        }

    }
}
