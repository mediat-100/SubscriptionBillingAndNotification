using Microsoft.AspNetCore.Mvc;

namespace SubscriptionBillingAndNotification.Controllers
{
    public class AdminController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
