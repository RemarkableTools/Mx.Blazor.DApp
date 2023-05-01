using Microsoft.AspNetCore.Components;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.Blazor.DApp.Shared.Connection;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using Mx.NET.SDK.Domain.Data.Block;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;

namespace Mx.Blazor.DApp.Client.Services
{
    public class NativeAuthService
    {
        private readonly NavigationManager _navigationManager;
        public NativeAuthService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public async Task<string> GenerateToken()
        {
            var host = Escape(Converter.ToBase64String(_navigationManager.BaseUri.GetHost()));

            Dictionary<string, string> @params = new()
            {
                {"shard","2" },
                {"fields","hash,timestamp" }
            };
            var block = Blocks.From(await Provider.GetBlocks(1, 0, @params))[0];
            var blockHash = Escape(Converter.ToBase64String(block.Hash));

            var ttl = NATIVE_AUTH_TTL;

            var extraInfo = new ExtraInfo(block.CreationDate.ToTimestamp());
            var extra = Escape(Converter.ToBase64String(JsonWrapper.Serialize(extraInfo)));

            return $"{host}.{blockHash}.{ttl}.{extra}";
        }

        private static string Escape(string str)
            => str.Replace('+', '-').Replace('/', '_').Replace("=", "");
    }
}
