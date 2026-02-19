using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [Authorize]
    public class SubscriptionsController : BaseController
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [Route("Details")]
        public async Task<ActionResult> GetSubscription(long id, CancellationToken cancellationToken)
        {
            var response = await _subscriptionService.GetSubscriptionById(id, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubscription(AddSubscriptionRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _subscriptionService.AddSubscription(request, cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubscription(UpdateSubscriptionRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _subscriptionService.UpdateSubscription(request, cancellationToken);
            return Ok(response);
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubscription(long id, CancellationToken cancellationToken)
        {
            var response = await _subscriptionService.DeleteSubscription(id, cancellationToken);
            return Ok(response);
        }

    }
}
