using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    [Authorize]
    public class WalletsController : BaseController
    {
        private readonly IWalletService _walletService;
        private long UserId => long.Parse(GetCurrentUserId())!;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        [Route("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateWallet(long userId, CancellationToken ct)
        {
            var wallet = await _walletService.CreateWallet(userId, ct);
            return Ok(wallet);
        }

        [HttpGet]
        [Route("Details")]
        public async Task<IActionResult> GetWallet(CancellationToken ct)
        {
            var response = await _walletService.GetWalletByUserId(UserId, ct);
            return Ok(response);
        }

        [HttpPost]
        [Route("Deposit")]
        public async Task<IActionResult> Deposit(AddFundsRequestDto request, CancellationToken ct)
        {
            var response = await _walletService.AddFunds(request, UserId, ct);
            return Ok(response);
        }

        [HttpGet]
        [Route("Transaction")]
        public async Task<IActionResult> GetTransaction(long transactionId, CancellationToken ct)
        {
            var response = await _walletService.GetTransaction(transactionId, ct);
            return Ok(response);
        }

        [HttpGet]
        [Route("TransactionHistory")]
        public async Task<IActionResult> GetTransactionHistory(int page = 1, int pageSize = 10)
        {
            var response = await _walletService.GetTransactionHistory(UserId, page, pageSize);
            return Ok(response);
        }
    }
}
