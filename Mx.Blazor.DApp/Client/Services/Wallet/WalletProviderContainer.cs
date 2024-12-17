using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserLocalStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.Blazor.DApp.Client.Application.Constants;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain;
using System.Text;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain.Helper;
using static Mx.NET.SDK.NativeAuthClient.NativeAuthClient;
using static Mx.NET.SDK.NativeAuthServer.Entities.NativeAuthToken;

namespace Mx.Blazor.DApp.Client.Services.Wallet
{
    public class WalletProviderContainer
    {
        private readonly IHttpService _http;
        private readonly IJSRuntime _jsRuntime;
        private readonly ISyncLocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly TransactionsContainer _transactionsContainer;
        private readonly NativeAuthService _nativeAuthService;

        public WalletProviderContainer(
            IHttpService httpService,
            IJSRuntime jsRuntime,
            ISyncLocalStorageService localStorage,
            NavigationManager navigationManager,
            TransactionsContainer transactionsContainer,
            NativeAuthService nativeAuthService)
        {
            _http = httpService;
            _jsRuntime = jsRuntime;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
            _transactionsContainer = transactionsContainer;
            _nativeAuthService = nativeAuthService;

            OnXPortalClientDisconnected += WalletDisconnected;

            Initialize();
        }

        private IWalletProvider? _walletProvider;

        private static event Action? OnXPortalClientDisconnected;
        [JSInvokable]
        public static void XPortalClientDisconnect() => OnXPortalClientDisconnected?.Invoke();

        private string? _authToken;
        public string WalletAddress => _localStorage.GetItemAsString(BrowserLocalStorage.WalletAddress) ?? "";
        private string WalletUrl => _localStorage.GetItemAsString(WebWalletUrl) ?? "";

        public event Action? OnWalletConnected;
        public event Action? OnWalletDisconnected;

        private async Task ValidateWalletConnection(AccountToken? accountToken)
        {
            if (IsConnected() || _accessTokenExpired) return; //because xPortal wallet always validates at refresh
            if (accountToken == null || !accountToken.IsValid())
            {
                _walletProvider = default!;
                _localStorage.RemoveItem(WalletProviderType);
                return;
            }

            var accessToken = GetAccessToken(accountToken.Address, _authToken, accountToken.Signature);
            _authToken = null;

            await ValidateWalletConnection(accessToken);
        }

        private async Task ValidateWalletConnection(string accessToken)
        {
            try
            {
                var connectionToken = await _http.PostAsync<ConnectionToken>(
                    $"api/connection/verify",
                    new AccessToken(accessToken)
                );
                _localStorage.SetItemAsString(BrowserLocalStorage.AccessToken, connectionToken.AccessToken);
                _localStorage.SetItem(AccessTokenExpires, DateTime.Now.AddSeconds(NativeAuthTtl).ToTimestamp());
                _localStorage.SetItemAsString(BrowserLocalStorage.WalletAddress, connectionToken.AccountToken.Address);

                OnWalletConnected?.Invoke();
            }
            catch (HttpException hex) //access token could not be generated
            {
                _walletProvider = default!;
                _localStorage.RemoveItem(WalletProviderType);

                await _jsRuntime.InvokeVoidAsync("alert", hex.Message);
            }
        }

        private void WalletDisconnected()
        {
            _walletProvider = default!;
            _accessTokenExpired = false;
            _localStorage.RemoveItem(BrowserLocalStorage.AccessToken);
            _localStorage.RemoveItem(AccessTokenExpires);
            _localStorage.RemoveItem(BrowserLocalStorage.WalletAddress);
            _localStorage.RemoveItem(WalletProviderType);
            _localStorage.RemoveItem(WebWalletUrl);
            _localStorage.RemoveAllWcItems();

            OnWalletDisconnected?.Invoke();
        }

        private bool _accessTokenExpired;

        private void Initialize()
        {
            _accessTokenExpired = false;
            switch (_localStorage.GetItem<WalletType>(WalletProviderType))
            {
                case WalletType.Extension:
                    _walletProvider = new ExtensionWalletProvider(_jsRuntime);
                    break;
                case WalletType.XPortal:
                    _walletProvider = new XPortalWalletProvider(_jsRuntime);
                    break;
                case WalletType.Hardware:
                    _walletProvider = new HardwareWalletProvider(_jsRuntime);
                    break;
                case WalletType.Web:
                    _walletProvider = new CrossWindowWalletProvider(_jsRuntime);
                    break;
                case WalletType.WebView:
                    _walletProvider = new WebViewProvider(_jsRuntime);
                    break;
                case WalletType.MetaMask:
                    _walletProvider = new MetaMaskWalletProvider(_jsRuntime);
                    break;
                default:
                    _localStorage.RemoveItem(BrowserLocalStorage.AccessToken);
                    _localStorage.RemoveItem(AccessTokenExpires);
                    _localStorage.RemoveItem(BrowserLocalStorage.WalletAddress);
                    _localStorage.RemoveItem(WalletProviderType);
                    _localStorage.RemoveItem(WebWalletUrl);
                    _localStorage.RemoveAllWcItems();
                    break;
            }

            var expireTimestamp = _localStorage.GetItem<long>(AccessTokenExpires);
            if (expireTimestamp <= 0 || expireTimestamp >= DateTime.Now.ToTimestamp())
                return;

            _accessTokenExpired = true;
            _localStorage.RemoveItem(BrowserLocalStorage.AccessToken);
        }

        public async Task InitializeAsync()
        {
            var accessToken = await _jsRuntime.InvokeAsync<string>("getAccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                await ConnectToWebView(accessToken);
                return;
            }

            if (_walletProvider is null) return;

            switch (_localStorage.GetItem<WalletType>(WalletProviderType))
            {
                case WalletType.Extension:
                    await _walletProvider.Init(WalletAddress);
                    break;
                case WalletType.XPortal:
                    await _walletProvider.Init();
                    break;
                case WalletType.Hardware:
                    await _walletProvider.Init();
                    break;
                case WalletType.Web:
                    await _walletProvider.Init(WalletUrl, WalletAddress);
                    break;
                case WalletType.WebView:
                    break;
                case WalletType.MetaMask:
                    await _walletProvider.Init(WalletUrl, WalletAddress);
                    break;
            }

            if (_accessTokenExpired)
            {
                WalletManagerService.Logout();
            }
        }

        public async Task ConnectToExtensionWallet()
        {
            _walletProvider = new ExtensionWalletProvider(_jsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                await _walletProvider.Init();
                var accountToken = await _walletProvider.Login(_authToken);
                await ValidateWalletConnection(accountToken);
            }
            catch
            {
                // ignored
            }
        }

        public async Task ConnectToXPortalWallet()
        {
            _walletProvider = new XPortalWalletProvider(_jsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                await _walletProvider.Init();
                // await WalletProvider.Login(_authToken);
                var accountToken = await _walletProvider.Login(_authToken);
                await ValidateWalletConnection(accountToken);
            }
            catch
            {
                // ignored
            }
        }

        public async Task ConnectToHardwareWallet(string authToken)
        {
            _walletProvider = new HardwareWalletProvider(_jsRuntime);

            try
            {
                _authToken = authToken;
                //Init is previously called from modal
                var accountToken = await _walletProvider.Login(_authToken);
                await ValidateWalletConnection(accountToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ConnectToCrossWindowWallet(string webWalletAddress)
        {
            _walletProvider = new CrossWindowWalletProvider(_jsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                _localStorage.SetItemAsString(WebWalletUrl, webWalletAddress);
                await _walletProvider.Init(webWalletAddress);
                var accountToken = await _walletProvider.Login(_authToken);
                await ValidateWalletConnection(accountToken);
            }
            catch
            {
                // ignored
            }
        }

        private async Task ConnectToWebView(string accessToken)
        {
            _walletProvider = new WebViewProvider(_jsRuntime);
            _localStorage.SetItem(WalletProviderType, WalletType.WebView);
            var parts = accessToken.Split('.');
            _authToken = DecodeValue(parts[1]);
            var accountToken = new AccountToken()
            {
                Address = DecodeValue(parts[0]),
                Signature = parts[2]
            };
            _navigationManager.NavigateTo(_navigationManager.Uri.GetUrlWithoutParameters());
            await _walletProvider.Init();
            await ValidateWalletConnection(accountToken);
        }

        public async Task ConnectToMetaMaskWallet(string webWalletAddress)
        {
            _walletProvider = new MetaMaskWalletProvider(_jsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                _localStorage.SetItemAsString(WebWalletUrl, webWalletAddress);
                await _walletProvider.Init(webWalletAddress);
                var accountToken = await _walletProvider.Login(_authToken);
                await ValidateWalletConnection(accountToken);
            }
            catch
            {
                // ignored
            }
        }

        public bool IsConnected()
        {
            if (_walletProvider is null) return false;

            try
            {
                var accessToken = _localStorage.GetItemAsString(BrowserLocalStorage.AccessToken);
                return !string.IsNullOrEmpty(accessToken);
            }
            catch
            {
                return false;
            }
        }

        public async Task Logout()
        {
            if (_walletProvider is null) return;

            await _walletProvider.Logout();
            WalletDisconnected();
        }

        public async Task<bool?> SignMessage(string messageText)
        {
            if (_walletProvider is null) return null;

            var signature = await _walletProvider.SignMessage(messageText);
            if (string.IsNullOrEmpty(signature))
            {
                return null;
            }

            var message = new Message(
                WalletAddress,
                Encoding.UTF8.GetBytes(messageText),
                Converter.FromHexString(signature)
            );
            return await _http.PostAsync<bool>("api/wallet/verify", message);
        }

        public async Task<string[]?> SignAndSendTransactions(
            string title = "Transaction(s)",
            params TransactionRequest[] transactionsRequest)
        {
            if (_walletProvider is null) return null;

            var signedTransactions = await _walletProvider.SignTransactions(transactionsRequest);
            if (string.IsNullOrEmpty(signedTransactions))
            {
                await _jsRuntime.InvokeVoidAsync("cancelTxToast");
                return null;
            }

            try
            {
                var transactions = JsonWrapper.Deserialize<TransactionRequestDto[]>(signedTransactions);
                var response = await Provider.SendTransactions(transactions);
                _transactionsContainer.NewTransactions(title, response.TxsHashes.Select(tx => tx.Value).ToArray());
                return response.TxsHashes.Select(tx => tx.Value).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string[]?> SignAndSendTransactions(
            string title = "Transaction(s)",
            params TransactionRequestDto[] transactionsRequest)
        {
            if (_walletProvider is null) return null;

            var signedTransactions = await _walletProvider.SignTransactions(transactionsRequest);
            if (string.IsNullOrEmpty(signedTransactions))
            {
                await _jsRuntime.InvokeVoidAsync("cancelTxToast");
                return null;
            }

            try
            {
                var transactions = JsonWrapper.Deserialize<TransactionRequestDto[]>(signedTransactions);
                var response = await Provider.SendTransactions(transactions);
                _transactionsContainer.NewTransactions(title, response.TxsHashes.Select(tx => tx.Value).ToArray());
                return response.TxsHashes.Select(tx => tx.Value).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task CancelAction()
        {
            if (_walletProvider is null) return;

            await _walletProvider.CancelAction();
        }
    }
}
