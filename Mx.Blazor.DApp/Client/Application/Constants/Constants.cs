namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public static class DAppConstants
    {
        public const int TxRemovalTime = 120; //seconds for each TX to be present in localstorage
        public const int TxDismissTime = 15000; //milliseconds for each successful TX until dismissed
        public const int InactivityTimer = 15; //minutes until disconnected from wallet
        public const int NativeAuthTtl = 14400; //seconds for auth token availability
    }

    public static class BrowserLocalStorage
    {
        public const string AccessToken = "accessToken";
        public const string AccessTokenExpires = "accessTokenExpires";
        public const string WalletAddress = "walletAddress";
        public const string WalletProviderType = "walletType";
        public const string WebWalletUrl = "webWalletUrl";
        public const string WalletConnectDefStorage = "walletconnect";
    }

    public static class BrowserSessionStorage
    {
        public const string TxList = "txs";
        public const string MobileDevice = "mobileDevice";
        public const string ExtensionAvailable = "extensionAvailable";
        public const string MetamaskAvailable = "metaMaskAvailable";
    }

    public enum WalletType
    {
        Extension = 1,
        XPortal,
        Hardware,
        Web,
        WebView,
        MetaMask
    }
}
