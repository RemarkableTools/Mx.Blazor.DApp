using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Domain.Data.Account;

namespace Mx.Blazor.DApp.Client.Services.Containers
{
    public class AccountContainer
    {
        public Account? Account;
        public AccountToken[]? AccountTokens;

        public async Task Initialize(string address)
        {
            if (Account != null && Account.Address.Bech32 != address)
            {
                Account = null;
                AccountTokens = null;
            }

            try
            {
                Account ??= Account.From(await Provider.GetAccount(address));
                AccountTokens ??= AccountToken.From(await Provider.GetAccountTokens(address, 1000));
            }
            catch { }
        }

        public async Task Sync()
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
            catch { }
        }
    }
}
