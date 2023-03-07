namespace Mx.Blazor.DApp.Client.Application.Constants
{
    public static class DappConstants
    {
        public const int TX_REMOVAL_TIME = 120; //seconds for each TX to be present in localstorage
    }

    public static class BrowserStorage
    {
        public const string ACCESS_TOKEN = "accessToken";
        public const string AUTH_TOKEN = "authToken";
        public const string ACCOUNT_TOKEN = "accountToken";
        public const string WALLET_TYPE = "wallettype";
        public const string WEB_WALLET_STATE = "webwalletstate";
        public const string SIG_MESSAGE = "sigmessage";
        public const string TX_TITLE = "txtitle";
        public const string WALLET_CONNECT_DEF_STORAGE = "walletconnect";

        public const string TX_LIST = "txs";
        public const string MOBILE_DEVICE = "mobiledevice";
        public const string EXTENSION_AVAILABLE = "extensionAvailable";
    }

    public enum WalletType
    {
        Extension= 1,
        XPortal,
        Hardware,
        Web
    }

    public enum WebWalletState
    {
        None = 1,
        LoggingIn,
        WaitingForSig,
        WaitingForTx
    }
}
