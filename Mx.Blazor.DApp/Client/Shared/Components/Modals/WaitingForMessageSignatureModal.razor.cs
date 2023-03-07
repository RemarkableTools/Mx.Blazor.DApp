using Mx.Blazor.DApp.Client.Application.Constants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserStorage;

namespace Mx.Blazor.DApp.Client.Shared.Components.Modals
{
    public partial class WaitingForMessageSignatureModal
    {
        private bool CanCancel { get; set; }

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += WalletConnected;

            SetCancelButton();
        }

        private void SetCancelButton()
        {
            if (SessionStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Extension)
                CanCancel = true;
            else
                CanCancel = false;
            StateHasChanged();
        }

        private void WalletConnected()
        {
            SetCancelButton();
        }

        public async void CancelAction()
        {
            await WalletProvider.CancelAction();
        }
    }
}
