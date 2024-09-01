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
    }
}