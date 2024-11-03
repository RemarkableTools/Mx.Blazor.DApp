using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Domain;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces
{
    public interface IWalletProvider
    {
        Task Init(params string[] args);
        Task<AccountToken> Login(string authToken);
        Task Logout();
        Task<string> SignMessage(string message);
        Task<string> SignTransaction(TransactionRequest transactionRequest);
        Task<string> SignTransactions(TransactionRequest[] transactionsRequest);
        Task CancelAction();
    }
}
