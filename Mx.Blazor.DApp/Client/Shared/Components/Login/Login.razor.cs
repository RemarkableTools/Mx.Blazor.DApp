using Microsoft.JSInterop;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserSessionStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;

namespace Mx.Blazor.DApp.Client.Shared.Components.Login
{
    public partial class Login
    {
        private enum LedgerStates
        {
            InitError,
            TokenError,
            EmptyAddressesList,
            List,
            Verify,
            Loading
        };

        private bool _isMobile = false;
        private bool _extensionWalletAvailable = false;
        private bool _metaMaskWalletAvailable = false;

        private string _walletConnectUri = "";

        private int LedgerAddressIndex { get; set; } = -1;
        private int _pageNumber = 0;
        private const int PAGE_SIZE = 10;
        private LedgerStates _ledgerState;
        private string? AuthToken { get; set; }
        private List<string>? Addresses { get; set; }

        protected override void OnInitialized()
        {
            _isMobile = SessionStorage.GetItem<bool>(MobileDevice);
            _extensionWalletAvailable = SessionStorage.GetItem<bool>(ExtensionAvailable);
            _metaMaskWalletAvailable = SessionStorage.GetItem<bool>(MetamaskAvailable);
        }

        public async void ExtensionWalletLogin()
        {
            await WalletProvider.ConnectToExtensionWallet();
        }

        public async void XPortalWalletLogin()
        {
            await WalletProvider.ConnectToXPortalWallet();
        }

        private void XPortalLogin()
        {
            NavigationManager.NavigateTo(_walletConnectUri);
        }

        public async void WebWalletLogin()
        {
            await WalletProvider.ConnectToCrossWindowWallet(Provider.NetworkConfiguration.WebWalletUri.AbsoluteUri);
        }

        public async void XAliasWalletLogin()
        {
            await WalletProvider.ConnectToCrossWindowWallet(Provider.NetworkConfiguration.XAliasWalletUri.AbsoluteUri);
        }

        public async void MetaMaskWalletLogin()
        {
            if (MetaMaskSnapWalletUrl == string.Empty)
            {
                await JsRuntime.InvokeVoidAsync("alert", "Network is not supported");
                return;
            }

            await WalletProvider.ConnectToMetaMaskWallet(MetaMaskSnapWalletUrl);
        }

        private void SetLedgerState(LedgerStates state)
        {
            _ledgerState = state;
            StateHasChanged();
        }

        public async void HardwareWalletLogin()
        {
            _pageNumber = 0;
            LedgerAddressIndex = -1;
            SetLedgerState(LedgerStates.Loading);
            Addresses = default!;

            var initialized = await JsRuntime.InvokeAsync<bool>("HardwareWallet.Obj.prepLedger");
            if (!initialized)
            {
                SetLedgerState(LedgerStates.InitError);
                return;
            }

            await ReadLedgerAddresses(_pageNumber, PAGE_SIZE);
        }

        private async Task ReadLedgerAddresses(int pageNumber, int pageSize)
        {
            SetLedgerState(LedgerStates.Loading);

            Addresses = await JsRuntime.InvokeAsync<List<string>?>(
                "HardwareWallet.Obj.getLedgerAddresses",
                pageNumber,
                pageSize
            );

            SetLedgerState(Addresses is null ? LedgerStates.EmptyAddressesList : LedgerStates.List);
        }

        private async void Prev()
        {
            if (_pageNumber <= 0)
                return;

            LedgerAddressIndex = -1;
            _pageNumber--;
            await ReadLedgerAddresses(_pageNumber, PAGE_SIZE);
        }

        private async void Next()
        {
            if (Addresses == null)
                return;

            LedgerAddressIndex = -1;
            _pageNumber++;
            await ReadLedgerAddresses(_pageNumber, PAGE_SIZE);
        }

        public void SetLedgerAddressIndex(int index)
        {
            LedgerAddressIndex = index + (_pageNumber * PAGE_SIZE);
            StateHasChanged();
        }

        private async void HardwareWalletConfirm()
        {
            if (LedgerAddressIndex == -1) return;

            try
            {
                AuthToken = await NativeAuth.GenerateToken();
                SetLedgerState(LedgerStates.Verify);
                await WalletProvider.ConnectToHardwareWallet(AuthToken);
                AuthToken = null;
            }
            catch
            {
                SetLedgerState(LedgerStates.TokenError);
            }
        }
    }
}
