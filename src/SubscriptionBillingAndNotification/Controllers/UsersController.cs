using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

       

        [HttpGet]
        [Route("User")]
        public async Task<IActionResult> Get(long id)
        {
            var response = await _userService.GetUserById(id);
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAllUsers(int pageNumber = 1, int pageSize = 10)
        {
            var response = _userService.GetAllUsers(pageNumber, pageSize);
            return Ok(response);
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string? email, int status, int pageSize = 1, int pageNumber = 10)
        {
            var response = _userService.SearchUsers(email, status, pageSize: pageSize, pageNumber: pageNumber);
            return Ok(response);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _userService.DeleteUser(id);
            return Ok(response);
        }

      


    }
}
