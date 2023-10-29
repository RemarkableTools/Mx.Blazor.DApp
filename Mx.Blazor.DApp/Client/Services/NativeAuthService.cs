using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.NET.SDK.NativeAuthClient;
using Mx.NET.SDK.NativeAuthClient.Entities;

namespace Mx.Blazor.DApp.Client.Services
{
    public class NativeAuthService
    {
        private readonly NavigationManager _navigationManager;
        private readonly NativeAuthClient _nativeAuthClient;
        public NativeAuthService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _nativeAuthClient = new(new NativeAuthClientConfig()
            {
                Origin = _navigationManager.BaseUri.GetHost(),
                ApiUrl = Provider.NetworkConfiguration.APIUri.AbsoluteUri,
                ExpirySeconds = NATIVE_AUTH_TTL,
                BlockHashShard = 2
            });
        }

        public async Task<string> GenerateToken()
        {
            return await _nativeAuthClient.GenerateToken();
        }
    }
}
