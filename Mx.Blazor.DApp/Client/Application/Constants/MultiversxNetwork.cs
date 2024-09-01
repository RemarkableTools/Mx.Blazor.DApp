using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public class MultiversxNetwork
    {
        public static Network MvxNetwork
        {
            //get => Network.MainNet;
            get => Network.DevNet;
            //get => Network.TestNet;
        }

        public static ApiProvider Provider
        {
            get => new(new ApiNetworkConfiguration(MvxNetwork));
        }

        public static TimeSpan TxCheckTime
        {
            get => TimeSpan.FromSeconds(6);
        }

        public static NetworkConfig NetworkConfig { get; set; } = default!;
        public async static Task InitializeNetworkConfig()
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