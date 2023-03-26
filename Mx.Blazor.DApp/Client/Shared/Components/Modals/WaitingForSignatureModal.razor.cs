using Mx.Blazor.DApp.Client.Application.Constants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.WalletType;

namespace Mx.Blazor.DApp.Client.Shared.Components.Modals
{
    public partial class WaitingForSignatureModal
    {
        private bool CanCancel { get; set; }

        protected override void OnInitialized()
        {
            WalletProvider.OnWalletConnected += WalletConnected;

            SetCancelButton();
        }

        private void SetCancelButton()
        {
            if (SessionStorage.GetItem<WalletType>(WALLET_TYPE) == Extension)
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
