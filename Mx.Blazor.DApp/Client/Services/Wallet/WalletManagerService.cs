using Microsoft.JSInterop;
using Mx.Blazor.DApp.Client.Services.Containers;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;

namespace Mx.Blazor.DApp.Client.Services.Wallet
{
    public class WalletManagerService
    {
        private readonly WalletProviderContainer _walletProvider;
        private readonly IJSRuntime JsRuntime;
        private readonly AccountContainer _accountContainer;
        public WalletManagerService(
            WalletProviderContainer walletProvider,
            IJSRuntime jsRuntime,
            AccountContainer accountContainer)
        {
            _walletProvider = walletProvider;
            JsRuntime = jsRuntime;
            _accountContainer = accountContainer;
        }

        private static event Action? OnDisconnectByInactivity;
        [JSInvokable]
        public static void DisconnectByInactivity() => OnDisconnectByInactivity?.Invoke();

        private static event Action? OnLogout;
        public static void Logout() => OnLogout?.Invoke();

        public async Task InitializeAsync()
        {
            OnDisconnectByInactivity += DisconnectEvent;
            OnLogout += LogoutFromWallet;
            await JsRuntime.InvokeVoidAsync("initInactivityTimer", INACTIVITY_TIMER);
        }

        public async void DisconnectEvent()
        {
            await Disconnect();
        }

        public async void LogoutFromWallet()
        {
            await Disconnect();
            await JsRuntime.InvokeVoidAsync("removeInactivityTimer");
        }

        private async Task Disconnect()
        {
            await _walletProvider.Logout();
            _accountContainer.Clear();

            OnDisconnectByInactivity -= DisconnectEvent;
            OnLogout -= LogoutFromWallet;
        }
    }
}
