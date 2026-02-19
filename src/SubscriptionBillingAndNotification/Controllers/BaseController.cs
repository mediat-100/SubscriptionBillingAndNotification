using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SubscriptionBillingAndNotification.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Name);
        }

        //protected bool IsAdmin {  get; set; } = false;
    }
}
