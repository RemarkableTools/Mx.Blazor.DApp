using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Wallet.Wallet;

namespace Mx.Blazor.DApp.Server.Helpers
{
    public static class SignatureVerifier
    {
        public static bool Verify(ConnectionRequest request)
        {
            var verifier = WalletVerifier.FromAddress(Address.FromBech32(request.AccountToken.Address));

            var message = new SignableMessage()
            {
                Message = $"{request.AccountToken.Address}{request.AuthToken}{{}}",
                Signature = request.AccountToken.Signature
            };

            return verifier.Verify(message);
        }
    }
}
