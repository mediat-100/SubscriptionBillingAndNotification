using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;

namespace SubscriptionBillingAndNotificationCore.Contracts.IService
{
    public interface IWalletService
    {
        Task<BaseResponse<WalletResponseDto>> CreateWallet(long userId, CancellationToken ct);
        Task<BaseResponse<WalletResponseDto>> GetWalletByUserId(long userId, CancellationToken ct);
        Task<BaseResponse<TransactionResponseDto>> AddFunds(AddFundsRequestDto request, long userId, CancellationToken ct);
        Task<BaseResponse<PagedResponse<TransactionResponseDto>>> GetTransactionHistory(long userId, int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<BaseResponse<TransactionResponseDto>> GetTransaction(long transactionId, CancellationToken ct);
        Task<BaseResponse<TransactionResponseDto>> DeductFunds(DeductFundsRequestDto request, long UserId, CancellationToken ct);
    }
}
