using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.NativeAuthClient;
using Mx.NET.SDK.NativeAuthClient.Entities;

namespace Mx.Blazor.DApp.Client.Services
{
    public class NativeAuthService(NavigationManager navigationManager)
    {
        private readonly NativeAuthClient _nativeAuthClient = new(new NativeAuthClientConfig()
        {
            Origin = navigationManager.BaseUri.GetHost(),
            ApiUrl = Provider.NetworkConfiguration.APIUri.AbsoluteUri,
            ExpirySeconds = NativeAuthTtl,
            BlockHashShard = 2
        });

        public async Task<string> GenerateToken()
        {
            return await _nativeAuthClient.GenerateToken();
        }
    }
}
