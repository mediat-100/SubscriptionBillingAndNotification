using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private long UserId => long.Parse(GetCurrentUserId())!;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

       

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var response = await _userService.GetUserById(UserId, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        [Route("Search")]
        [Authorize(Roles = "Admin")]
        public IActionResult Search(string? email, int? status, int? userType, int pageNumber = 1, int pageSize = 10)
        {
            var response = _userService.SearchUsers(email, status, userType, pageNumber, pageSize);
            return Ok(response);
        }

        [HttpDelete]
        [Route("Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            var response = await _userService.DeleteUser(id, cancellationToken);
            return Ok(response);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(UpdateUserRequestDto request, CancellationToken cancellationToken)
        {
            var response = await _userService.UpdateUser(request, cancellationToken);
            return Ok(response);
        }


    }
}
