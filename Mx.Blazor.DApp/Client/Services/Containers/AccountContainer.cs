﻿using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Domain.Data.Accounts;

namespace Mx.Blazor.DApp.Client.Services.Containers
{
    public class AccountContainer
    {
        public Account? Account { get; private set; }
        public AccountToken[]? AccountTokens { get; private set; }

        public async Task Initialize(string address)
        {
            if (Account != null) return;

            try
            {
                Account ??= Account.From(await Provider.GetAccount(address));
                AccountTokens ??= AccountToken.From(await Provider.GetAccountTokens(address, 1000));
            }
            catch
            {
                // ignored
            }
        }

        public void Clear()
        {
            Account = default!;
            AccountTokens = default!;
        }

        public async Task SyncAll()
        {
            await SyncAccount();
            await SyncAccountTokens();
        }

        public async Task SyncAccount()
        {
            if (Account is null) return;

            await Account.Sync(Provider);
        }

        public async Task SyncAccountTokens()
        {
            if (Account is null) return;

            try
            {
                AccountTokens = AccountToken.From(await Provider.GetAccountTokens(Account.Address.Bech32, 1000));
            }
            catch
            {
                // ignored
            }
        }
    }
}
