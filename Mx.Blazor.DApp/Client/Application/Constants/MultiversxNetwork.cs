using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Domain.Data.Network;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public class MultiversxNetwork
    {
        public static ApiProvider Provider
        {
            //get => new(new ApiNetworkConfiguration(Network.MainNet));
            get => new(new ApiNetworkConfiguration(Network.DevNet));
            //get => new(new ApiNetworkConfiguration(Network.TestNet));
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
    }
}