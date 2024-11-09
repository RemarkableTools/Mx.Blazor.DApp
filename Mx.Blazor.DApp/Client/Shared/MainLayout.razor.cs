using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.Configuration;

namespace Mx.Blazor.DApp.Client.Shared
{
    public partial class MainLayout
    {
        private bool WalletConnected;
        private string CurrentNetwork { get; } = "";

        public MainLayout()
        {
            CurrentNetwork = Provider.NetworkConfiguration.Network switch
            {
                Network.MainNet => "MainNet",
                Network.DevNet => "DevNet",
                Network.TestNet => "TestNet",
                _ => CurrentNetwork
            };
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
