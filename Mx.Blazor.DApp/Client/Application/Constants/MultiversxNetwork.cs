using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public class MultiversxNetwork
    {
        public static MultiversxProvider Provider
        {
            //get => new(new MultiversxNetworkConfiguration(Network.MainNet));
            get => new(new MultiversxNetworkConfiguration(Network.DevNet));
            //get => new(new MultiversxNetworkConfiguration(Network.TestNet));
        }

        public static TimeSpan TxCheckTime = TimeSpan.FromSeconds(6);
    }
}