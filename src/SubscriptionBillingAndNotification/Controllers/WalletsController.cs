using Microsoft.AspNetCore.Mvc;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;

namespace SubscriptionBillingAndNotification.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class WalletsController : Controller
    {
        private readonly IWalletService _walletService;

        public WalletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet]
        [Route("GetWalletByUserId")]
        public async Task<IActionResult> GetWalletByUserId(long userId, CancellationToken ct)
        {
            var response = await _walletService.GetWalletByUserId(userId, ct);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetWallet")]
        public async Task<IActionResult> GetWallet(long walletId, CancellationToken ct)
        {
            var response = await _walletService.GetWallet(walletId, ct);
            return Ok(response);
        }

        [HttpPost]
        [Route("AddFunds")]
        public async Task<IActionResult> AddFunds(AddFundsRequestDto request, CancellationToken ct)
        {
            var response = await _walletService.AddFunds(request, ct);
            return Ok(response);
        }

        [HttpPost]
        [Route("ProcessSubscriptionPayment")]
        public async Task<IActionResult> ProcessSubscriptionPayment(SubscriptionPaymentRequestDto request, CancellationToken ct)
        {
            var response = await _walletService.ProcessSubscriptionPayment(request, ct);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetTransaction")]
        public async Task<IActionResult> GetTransaction(long transactionId, CancellationToken ct)
        {
            var response = await _walletService.GetTransaction(transactionId, ct);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetTransactionHistory")]
        public async Task<IActionResult> GetTransactionHistory(long walletId, int page = 1, int pageSize = 10)
        {
            var response = await _walletService.GetTransactionHistory(walletId, page, pageSize);
            return Ok(response);
        }
    }
}
