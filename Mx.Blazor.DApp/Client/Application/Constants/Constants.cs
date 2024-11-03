namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public static class DAppConstants
    {
        public const int TX_REMOVAL_TIME = 120; //seconds for each TX to be present in localstorage
        public const int TX_DISMISS_TIME = 15000; //milliseconds for each successful TX until dismissed
        public const int INACTIVITY_TIMER = 15; //minutes until disconnected from wallet
        public const int NATIVE_AUTH_TTL = 14400; //seconds for auth token availability
    }

    public static class BrowserLocalStorage
    {
        public const string ACCESS_TOKEN = "accessToken";
        public const string ACCESS_TOKEN_EXPIRES = "accessTokenExpires";
        public const string AUTH_TOKEN = "authToken";
        public const string WALLET_ADDRESS = "walletAddress";
        public const string WALLET_TYPE = "walletType";
        public const string WEB_WALLET_STATE = "webWalletState";
        public const string WEB_WALLET_URL = "webWalletUrl";
        public const string WALLET_CONNECT_DEF_STORAGE = "walletconnect";
    }

    public static class BrowserSessionStorage
    {
        public const string POST_PROCESS = "postProcess";
        public const string POST_PROCESS_OBJECT = "postProcessObject";
        public const string SIG_MESSAGE = "sigmessage";
        public const string TX_TITLE = "txtitle";
        public const string TX_LIST = "txs";
        public const string MOBILE_DEVICE = "mobileDevice";
        public const string EXTENSION_AVAILABLE = "extensionAvailable";
        public const string METAMASK_AVAILABLE = "metaMaskAvailable";
    }

    public enum WalletType
    {
        Extension= 1,
        XPortal,
        Hardware,
        Web,
        WebView,
        CrossWindow,
        MetaMask
    }

    public enum WebWalletState
    {
        None = 1,
        LoggingIn,
        WaitingForSig,
        WaitingForTx
    }

    public enum PostTxSendProcess
    {
        ProcessID1 = 1,
    }
}
