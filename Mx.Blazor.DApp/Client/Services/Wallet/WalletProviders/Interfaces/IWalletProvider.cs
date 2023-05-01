﻿using Mx.NET.SDK.Domain;

namespace Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces
{
    public interface IWalletProvider
    {
        Task Init(params string[] args);
        Task<string> Login(string authToken);
        Task<string> GetAddress();
        Task<bool> IsConnected();
        Task Logout();
        Task TransactionIsCanceled();
        Task<string> SignMessage(string message);
        Task<string> SignTransaction(TransactionRequest transactionRequest);
        Task<string> SignTransactions(TransactionRequest[] transactionsRequest);
        Task CancelAction();
    }
}
