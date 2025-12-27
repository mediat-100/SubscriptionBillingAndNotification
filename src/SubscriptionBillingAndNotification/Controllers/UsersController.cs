using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

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
        [Route("Details")]
        public async Task<IActionResult> Get(long id)
        {
            var response = await _userService.GetUserById(id);
            return Ok(response);
        }

        [HttpGet]
        [Route("Search")]
        public IActionResult Search(string? email, int? status, int? userType, int pageNumber = 1, int pageSize = 10)
        {
            var response = _userService.SearchUsers(email, status, userType, pageNumber, pageSize);
            return Ok(response);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _userService.DeleteUser(id);
            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(UpdateUserRequestDto request)
        {
            var response = await _userService.UpdateUser(request);
            return Ok(response);
        }


    }
}
