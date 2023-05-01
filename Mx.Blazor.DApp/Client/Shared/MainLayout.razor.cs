using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Configuration;

namespace Mx.Blazor.DApp.Client.Shared
{
    public partial class MainLayout
    {
        private bool WalletConnected;
        private string CurrentNetwork { get; set; } = "";

        public MainLayout()
        {
            switch (Provider.NetworkConfiguration.Network)
            {
                case Network.MainNet:
                    CurrentNetwork = "MainNet";
                    break;
                case Network.DevNet:
                    CurrentNetwork = "DevNet";
                    break;
                case Network.TestNet:
                    CurrentNetwork = "TestNet";
                    break;
            }
        }

        protected override void OnInitialized()
        {
            WalletConnected = WalletProvider.IsConnected();

            WalletProvider.OnWalletConnected += OnWalletConnected;
            WalletProvider.OnWalletDisconnected += OnWalletDisconnected;
        }

        protected override async Task OnInitializedAsync()
        {
            await WalletProvider.InitializeAsync();

            if (WalletConnected)
                await InitializeConnection();
        }

        private async Task InitializeConnection()
        {
            await InitializeNetworkConfig();
            await WalletManager.InitializeAsync();
        }

        private async void OnWalletConnected()
        {
            WalletConnected = WalletProvider.IsConnected();
            StateHasChanged();

            await InitializeConnection();
        }

        private void OnWalletDisconnected()
        {
            WalletConnected = WalletProvider.IsConnected();
            StateHasChanged();
        }
    }
}
