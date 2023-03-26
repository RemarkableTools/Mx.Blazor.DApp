using Microsoft.AspNetCore.Components;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;

namespace Mx.Blazor.DApp.Client.Pages
{
    public partial class Transactions
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        private TransactionDto[]? AccountTransactions;

        public async Task GetTransactions()
        {
            var address = WalletProvider.GetAddress();
            Dictionary<string, string> dict = new()
            {
                { "sender", address }
            };
            AccountTransactions = await Provider.GetTransactions(100, 0, dict);

            StateHasChanged();
        }
    }
}
