using Mx.NET.SDK.Domain;

namespace Mx.Blazor.DApp.Client.Services.WalletProviders.Interfaces
{
    public interface IWalletProvider
    {
        Task Init(params string[] args);
        Task<string> Login(string authToken);
        Task<string> GetAddress();
        Task<bool> IsConnected();
        Task Logout();
        Task TransactionIsCanceled();
        Task<string> SignTransaction(TransactionRequest transactionRequest);
        Task<string> SignTransactions(TransactionRequest[] transactionsRequest);
    }
}
