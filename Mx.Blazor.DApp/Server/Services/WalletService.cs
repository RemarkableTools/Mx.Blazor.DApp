using Mx.NET.SDK.Core.Domain;
using Mx.NET.SDK.Wallet.Wallet;

namespace Mx.Blazor.DApp.Server.Services
{
    public interface IWalletService
    {
        bool Verify(SignableMessage message);
    }

    public class WalletService : IWalletService
    {
        public bool Verify(SignableMessage message)
        {
            var verifier = WalletVerifier.FromAddress(message.Address);
            return verifier.Verify(message);
        }
    }
}
