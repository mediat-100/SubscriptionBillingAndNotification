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
        Task<BaseResponse<WalletResponseDto>> GetWallet(long walletId, CancellationToken ct);
        Task<BaseResponse<WalletResponseDto>> GetWalletByUserId(long userId, CancellationToken ct);
        Task<BaseResponse<TransactionResponseDto>> AddFunds(AddFundsRequestDto request, CancellationToken ct);
        //Task<BaseResponse<string>> ProcessSubscriptionPayment(SubscriptionPaymentRequestDto subscriptionPaymentRequestDto, CancellationToken ct);
        Task<BaseResponse<PagedResponse<TransactionResponseDto>>> GetTransactionHistory(long walletId, int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<BaseResponse<TransactionResponseDto>> GetTransaction(long transactionId, CancellationToken ct);
        Task<BaseResponse<TransactionResponseDto>> DeductFunds(DeductFundsRequestDto request, CancellationToken ct);
    }
}
