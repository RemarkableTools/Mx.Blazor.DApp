using Mx.Blazor.DApp.Client.Application.Constants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserLocalStorage;

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
            CanCancel = LocalStorage.GetItem<WalletType>(WALLET_TYPE) switch
            {
                WalletType.Extension or WalletType.CrossWindow or WalletType.MetaMask => true,
                _ => false,
            };
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
