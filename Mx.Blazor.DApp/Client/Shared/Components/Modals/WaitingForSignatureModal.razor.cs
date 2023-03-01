using Mx.Blazor.DApp.Client.Application.Constants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserStorage;

namespace Mx.Blazor.DApp.Client.Shared.Components.Modals
{
    public partial class WaitingForSignatureModal
    {
        private bool CanCancel { get; set; }

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += WalletConnected;
        }

        private void WalletConnected()
        {
            if (SessionStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Extension)
                CanCancel = true;
            else
                CanCancel = false;
            StateHasChanged();
        }

        public async void CancelAction()
        {
            await WalletProvider.CancelAction();
        }
    }
}
