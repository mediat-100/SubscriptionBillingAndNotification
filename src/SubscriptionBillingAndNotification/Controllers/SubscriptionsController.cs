using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class SubscriptionsController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        [Route("Details")]
        public ActionResult GetSubscription(long id)
        {
            var response = _subscriptionService.GetSubscriptionById(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult CreateSubscription(AddSubscriptionRequestDto request)
        {
            var response = _subscriptionService.AddSubscription(request);
            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public IActionResult UpdateSubscription(UpdateSubscriptionRequestDto request)
        {
            var response = _subscriptionService.UpdateSubscription(request);
            return Ok(response);
        }

        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeleteSubscription(long id)
        {
            var response = _subscriptionService.DeleteSubscription(id);
            return Ok(response);
        }
    }
}
