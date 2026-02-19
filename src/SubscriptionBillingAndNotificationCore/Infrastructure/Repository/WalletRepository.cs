using Microsoft.EntityFrameworkCore;
using SubscriptionBillingAndNotificationCore.Contracts.IRepository;
using SubscriptionBillingAndNotificationCore.Entities;
using SubscriptionBillingAndNotificationCore.Infrastructure.Context;

namespace SubscriptionBillingAndNotificationCore.Infrastructure.Repository
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WalletRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<WalletTransaction> AddTransaction(WalletTransaction transaction, CancellationToken cancellationToken)
        {
            _dbContext.Add(transaction);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return transaction;
        }

        public async Task<Wallet> CreateWallet(Wallet wallet, CancellationToken cancellationToken)
        {
            _dbContext.Wallets.Add(wallet);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return wallet;
        }

        public async Task<IEnumerable<WalletTransaction>> GetTransactions(long walletId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var transactions = await _dbContext.WalletTransactions.Where(x => x.WalletId == walletId && !x.IsDeleted)
                .Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);

            return transactions.AsEnumerable();
        }

        public async Task<Wallet?> GetWalletByUserId(long userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Wallets.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
        }

        public async Task<Wallet> UpdateWallet(Wallet wallet, CancellationToken cancellationToken)
        {
            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return wallet;
        }
        public async Task<WalletTransaction?> GetTransaction(long transactionId, CancellationToken ct)
        {
            var transaction = await _dbContext.WalletTransactions.Where(x => x.Id == transactionId && !x.IsDeleted).FirstOrDefaultAsync(ct);
            return transaction;
        }
    }
}
