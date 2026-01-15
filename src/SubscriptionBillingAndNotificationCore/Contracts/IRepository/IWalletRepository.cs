using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionBillingAndNotificationCore.Entities;

namespace SubscriptionBillingAndNotificationCore.Contracts.IRepository
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetWalletById(long id, CancellationToken cancellationToken);
        Task<Wallet?> GetWalletByUserId(long userId, CancellationToken cancellationToken);
        Task<Wallet> CreateWallet(Wallet wallet, CancellationToken cancellationToken);
        Task<Wallet> UpdateWallet(Wallet wallet, CancellationToken cancellationToken);
        Task<IEnumerable<WalletTransaction>> GetTransactions(long walletId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<WalletTransaction> AddTransaction(WalletTransaction transaction, CancellationToken cancellationToken);
        Task<WalletTransaction?> GetTransaction(long transactionId, CancellationToken cancellationToken);
    }
}
