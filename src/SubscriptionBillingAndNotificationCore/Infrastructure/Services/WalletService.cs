using Microsoft.EntityFrameworkCore;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Contracts.IService;
using SubscriptionBillingAndNotificationCore.Dtos.Requests;
using SubscriptionBillingAndNotificationCore.Dtos.Responses;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserService _userService;

        public WalletService(ApplicationDbContext dbContext,IWalletRepository walletRepository, IUserService userService)
        {
            _dbContext = dbContext;
            _walletRepository = walletRepository;
            _userService = userService;
        }

        public async Task<BaseResponse<WalletResponseDto>> GetWalletByUserId(long userId, CancellationToken ct)
        {
            Wallet wallet = await _walletRepository.GetWalletByUserId(userId, ct) 
                ?? throw new NotFoundException($"Wallet for userId {userId} not found!");

            var response = MapWalletEntityToWalletResponseDto(wallet);

            return BaseResponse<WalletResponseDto>.Ok(response, "Wallet Fetched Successfully");
        }

        public async Task<BaseResponse<WalletResponseDto>> GetWallet(long walletId, CancellationToken ct)
        {
            Wallet wallet = await _walletRepository.GetWalletById(walletId, ct)
                ?? throw new NotFoundException("WalletId not found!");

            var response = MapWalletEntityToWalletResponseDto(wallet);

            return BaseResponse<WalletResponseDto>.Ok(response, "Wallet Fetched Successfully");
        }


        public async Task<BaseResponse<WalletResponseDto>> CreateWallet(long userId, CancellationToken ct)
        {
            var existingUserWallet = await _dbContext.Wallets.Where(x => x.UserId == userId && !x.IsDeleted).FirstOrDefaultAsync(ct);
            if (existingUserWallet != null)
                throw new ValidationException("Wallet alreday exist!");

            var wallet = new Wallet
            {
                UserId = userId,
                Balance = 50000, // set every user wallet to 50000 at creation
                Status = Enums.WalletStatus.Active,
            };
            wallet = await _walletRepository.CreateWallet(wallet, ct);
            var response = MapWalletEntityToWalletResponseDto(wallet);

            return BaseResponse<WalletResponseDto>.Ok(response, "Wallet Created Successfully");
        }
        public async Task<BaseResponse<TransactionResponseDto>> AddFunds(AddFundsRequestDto request, CancellationToken ct)
        {
            if (request.Amount <= 0)
                throw new ValidationException("Amount must be greater than 0");

            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var wallet = await _walletRepository.GetWalletById(request.WalletId, ct) ??
                        throw new NotFoundException("WalletId not found!");
                wallet.Balance += request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                var walletTransaction = new WalletTransaction
                {
                    WalletId = request.WalletId,
                    Amount = request.Amount,
                    Type = Enums.TransactionType.Credit,
                    Status = Enums.TransactionStatus.Completed,
                    BalanceAfter = wallet.Balance,
                    ReferenceId = "CREDIT" + DateTime.UtcNow.Millisecond.ToString(),
                    Description = "TOPUP"
                };

                _dbContext.Wallets.Update(wallet);
                _dbContext.WalletTransactions.Add(walletTransaction);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                var response = MapToTransactionResponse(walletTransaction);
                return BaseResponse<TransactionResponseDto>.Ok(response, "Funds added successfully");
            }
            catch 
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
           
            
        }

        public async Task<BaseResponse<TransactionResponseDto>> DeductFunds(DeductFundsRequestDto request, CancellationToken ct)
        {
            var wallet = await _walletRepository.GetWalletById(request.WalletId, ct) ??
                    throw new ValidationException("WalletId not found!");

            // check if wallet has enough funds for subscription
            if (wallet.Balance < request.Amount)
                throw new ValidationException("Insufficient funds!!");

            var walletTransaction = new WalletTransaction();
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                wallet.Balance -= request.Amount;
                wallet.UpdatedAt = DateTime.UtcNow;

                walletTransaction = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = request.Amount,
                    BalanceAfter = wallet.Balance,
                    Status = Enums.TransactionStatus.Completed,
                    Description = request.Description,
                    ReferenceId = "TRX" + DateTime.UtcNow.Ticks.ToString(),
                    Type = Enums.TransactionType.Debit,
                };

                _dbContext.WalletTransactions.Add(walletTransaction);
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }

            var transactionResponse = MapToTransactionResponse(walletTransaction);
            return BaseResponse<TransactionResponseDto>.Ok(transactionResponse, "Funds Deducted Successfully!");
        }

        public async Task<BaseResponse<TransactionResponseDto>> GetTransaction(long transactionId, CancellationToken ct)
        {
            var transaction = await _walletRepository.GetTransaction(transactionId, ct) ?? 
                throw new NotFoundException("TransactionId Not Found!");

            var response = MapToTransactionResponse(transaction);

            return BaseResponse<TransactionResponseDto>.Ok(response, "Transaction Fetched Successfully");
        }

        public async Task<BaseResponse<PagedResponse<TransactionResponseDto>>> GetTransactionHistory(long walletId, int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var transactions = await _walletRepository.GetTransactions(walletId, page, pageSize, ct);
            
            var transactionsResponseDto = transactions.Select(x => MapToTransactionResponse(x)).ToList() ?? new List<TransactionResponseDto>();
            var pagedresponse = PagedResponse<TransactionResponseDto>.Create(transactionsResponseDto.AsEnumerable(), page, pageSize);

            return BaseResponse<PagedResponse<TransactionResponseDto>>.Ok(pagedresponse, "Transactions Fetched Successfully");
        }

       

       

       

        private WalletResponseDto MapWalletEntityToWalletResponseDto(Wallet wallet)
        {
            return new WalletResponseDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Status = wallet.Status.ToString(),
                Currency = wallet.Currency,
                Balance = wallet.Balance,
                IsDeleted = wallet.IsDeleted,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
        }

        private TransactionResponseDto MapToTransactionResponse(WalletTransaction walletTransaction)
        {
            return new TransactionResponseDto
            {
                Id= walletTransaction.Id,
                WalletId = walletTransaction.WalletId,
                Amount = walletTransaction.Amount,
                BalanceAfter = walletTransaction.BalanceAfter,
                Status = walletTransaction.Status.ToString(),
                Description = walletTransaction.Description,
                ReferenceId = walletTransaction.ReferenceId,
                Type = walletTransaction.Type.ToString(),
                IsDeleted = walletTransaction.IsDeleted,
                CreatedAt = walletTransaction.CreatedAt,
                UpdatedAt = walletTransaction.UpdatedAt
            };
        }


    }
}
