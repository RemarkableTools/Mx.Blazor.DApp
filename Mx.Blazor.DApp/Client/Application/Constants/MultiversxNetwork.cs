using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public static class MultiversxNetwork
    {
        public static Network MvxNetwork =>
            Network.DevNet;
        // Network.MainNet;
        // Network.TestNet;

        public static ApiProvider Provider => new(new ApiNetworkConfiguration(MvxNetwork));

        public static TimeSpan TxCheckTime => TimeSpan.FromSeconds(6);

        public static NetworkConfig? NetworkConfig { get; private set; }

        public static async Task InitializeNetworkConfig()
        {
            NetworkConfig ??= await NetworkConfig.GetFromNetwork(Provider);
        }

        public static string MetaMaskSnapWalletUrl
        {
            get
            {
                return MvxNetwork switch
                {
                    Network.MainNet => "https://snap-wallet.multiversx.com",
                    Network.DevNet => "https://devnet-snap-wallet.multiversx.com",
                    _ => string.Empty
                };
            }
        }
    }
}
