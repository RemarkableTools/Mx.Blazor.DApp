using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Services.Containers;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;

namespace Mx.Blazor.DApp.Client.Services.Wallet
{
    public class WalletManagerService(
        WalletProviderContainer walletProvider,
        IJSRuntime jsRuntime,
        AccountContainer accountContainer)
    {
        private static event Action? OnDisconnectByInactivity;
        [JSInvokable]
        public static void DisconnectByInactivity() => OnDisconnectByInactivity?.Invoke();

        private static event Action? OnLogout;
        public static void Logout() => OnLogout?.Invoke();

        public async Task InitializeAsync()
        {
            OnDisconnectByInactivity += DisconnectEvent;
            OnLogout += LogoutFromWallet;
            await jsRuntime.InvokeVoidAsync("initInactivityTimer", InactivityTimer);
        }

        private async void DisconnectEvent()
        {
            await Disconnect();
        }

        private async void LogoutFromWallet()
        {
            await Disconnect();
            await jsRuntime.InvokeVoidAsync("removeInactivityTimer");
        }

        private async Task Disconnect()
        {
            await walletProvider.Logout();
            accountContainer.Clear();

            OnDisconnectByInactivity -= DisconnectEvent;
            OnLogout -= LogoutFromWallet;
        }
    }
}
