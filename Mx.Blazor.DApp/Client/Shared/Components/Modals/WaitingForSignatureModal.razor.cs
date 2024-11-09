using Mx.Blazor.DApp.Client.Application.Constants;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserLocalStorage;

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
            CanCancel = LocalStorage.GetItem<WalletType>(WalletProviderType) switch
            {
                WalletType.Extension or WalletType.Web or WalletType.MetaMask => true,
                _ => false,
            };
            StateHasChanged();
        }

        private void WalletConnected()
        {
            SetCancelButton();
        }

        private async void CancelAction()
        {
            await WalletProvider.CancelAction();
        }
    }
}
