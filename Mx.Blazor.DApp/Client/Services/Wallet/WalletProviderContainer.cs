using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Blazored.SessionStorage;
using Blazored.LocalStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserSessionStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserLocalStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.DAppConstants;
using static Mx.Blazor.DApp.Client.Application.Constants.MultiversxNetwork;
using Mx.Blazor.DApp.Client.Application.Constants;
using Mx.Blazor.DApp.Client.Application.ExtensionMethods;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain;
using Mx.Blazor.DApp.Client.Models;
using System.Text;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.Blazor.DApp.Client.Services.Containers;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders.Interfaces;
using Mx.Blazor.DApp.Client.Services.Wallet.WalletProviders;
using Mx.Blazor.DApp.Client.Application.Exceptions;
using Mx.NET.SDK.Domain.Helper;

namespace Mx.Blazor.DApp.Client.Services.Wallet
{
    public class WalletProviderContainer
    {
        private readonly IHttpService Http;
        private readonly IJSRuntime JsRuntime;
        private readonly ISyncLocalStorageService _localStorage;
        private readonly ISyncSessionStorageService _sessionStorage;
        private readonly NavigationManager NavigationManager;
        private readonly TransactionsContainer TransactionsContainer;
        private readonly PostTxSendService _postTxSendService;
        private readonly NativeAuthService _nativeAuthService;
        public WalletProviderContainer(
            IHttpService httpService,
            IJSRuntime jsRuntime,
            ISyncLocalStorageService localStorage,
            ISyncSessionStorageService sessionStorage,
            NavigationManager navigationManager,
            TransactionsContainer transactionsContainer,
            PostTxSendService postTxSendService,
            NativeAuthService nativeAuthService)
        {
            Http = httpService;
            JsRuntime = jsRuntime;
            _sessionStorage = sessionStorage;
            _localStorage = localStorage;
            NavigationManager = navigationManager;
            TransactionsContainer = transactionsContainer;
            _postTxSendService = postTxSendService;
            _nativeAuthService = nativeAuthService;

            OnXPortalClientConnected += ValidateWalletConnection;
            OnXPortalClientDisconnected += WalletDisconnected;

            Initialize();
        }

        private IWalletProvider WalletProvider = default!;

        private static Func<AccountToken?, Task> OnXPortalClientConnected = default!;
        private static event Action? OnXPortalClientDisconnected;
        [JSInvokable]
        public static async void XPortalClientConnect(string accountInfo) => await OnXPortalClientConnected.Invoke(JsonWrapper.Deserialize<AccountToken>(accountInfo));
        [JSInvokable]
        public static void XPortalClientDisconnect() => OnXPortalClientDisconnected?.Invoke();

        private string? _authToken;

        public event Action? OnWalletConnected;
        public event Action? OnWalletDisconnected;
        public async Task ValidateWalletConnection(AccountToken? accountToken)
        {
            if (IsConnected() || accessTokenExpired) return; //because xPortal wallet always validates at refresh
            if (accountToken == null || !accountToken.IsValid())
            {
                WalletProvider = default!;
                _localStorage.RemoveItem(WALLET_TYPE);

                await JsRuntime.InvokeVoidAsync("alert", "Wallet Token is not valid");
                return;
            }

            var connectionRequest = new ConnectionRequest()
            {
                AccountToken = accountToken,
                AuthToken = _authToken
            };
            _authToken = null;

            try
            {
                var connectionToken = await Http.PostAsync<ConnectionToken>("api/connection/verify", connectionRequest);
                _localStorage.SetItemAsString(ACCESS_TOKEN, connectionToken.AccessToken);
                _localStorage.SetItem(ACCESS_TOKEN_EXPIRES, DateTime.Now.AddSeconds(NATIVE_AUTH_TTL).ToTimestamp());
                _localStorage.SetItem(ACCOUNT_TOKEN, accountToken);

                OnWalletConnected?.Invoke();
            }
            catch (HttpException hex) //access token could no be generated
            {
                WalletProvider = default!;
                _localStorage.RemoveItem(WALLET_TYPE);

                await JsRuntime.InvokeVoidAsync("alert", hex.Message);
            }
        }
        public void WalletDisconnected()
        {
            WalletProvider = default!;
            accessTokenExpired = false;
            _localStorage.RemoveItem(ACCESS_TOKEN);
            _localStorage.RemoveItem(ACCESS_TOKEN_EXPIRES);
            _localStorage.RemoveItem(ACCOUNT_TOKEN);
            _localStorage.RemoveItem(WALLET_TYPE);
            _localStorage.RemoveItem(WEB_WALLET_STATE);
            _localStorage.RemoveAllWcItems();

            OnWalletDisconnected?.Invoke();
        }

        private bool accessTokenExpired;
        private void Initialize()
        {
            accessTokenExpired = false;
            switch (_localStorage.GetItem<WalletType>(WALLET_TYPE))
            {
                case WalletType.Extension:
                    WalletProvider = new ExtensionWalletProvider(JsRuntime);
                    break;
                case WalletType.XPortal:
                    WalletProvider = new XPortalWalletProvider(JsRuntime);
                    break;
                case WalletType.Hardware:
                    WalletProvider = new HardwareWalletProvider(JsRuntime);
                    break;
                case WalletType.Web:
                    WalletProvider = new WebWalletProvider(JsRuntime);
                    break;
                default:
                    _localStorage.RemoveItem(ACCESS_TOKEN);
                    _localStorage.RemoveItem(ACCESS_TOKEN_EXPIRES);
                    _localStorage.RemoveItem(ACCOUNT_TOKEN);
                    _localStorage.RemoveItem(WALLET_TYPE);
                    _localStorage.RemoveItem(WEB_WALLET_STATE);
                    _localStorage.RemoveAllWcItems();
                    break;
            }

            var expireTimestamp = _localStorage.GetItem<long>(ACCESS_TOKEN_EXPIRES);
            if (expireTimestamp > 0 && expireTimestamp < DateTime.Now.ToTimestamp())
            {
                accessTokenExpired = true;
                _localStorage.RemoveItem(ACCESS_TOKEN);
            }
        }

        public async Task InitializeAsync()
        {
            var accessToken = await JsRuntime.InvokeAsync<string>("getAccessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                await ConnectToWebView(accessToken);
                return;
            }

            if (WalletProvider is null) return;

            switch (_localStorage.GetItem<WalletType>(WALLET_TYPE))
            {
                case WalletType.Extension:
                    await WalletProvider.Init(GetAddress());
                    break;
                case WalletType.XPortal:
                    await WalletProvider.Init();
                    break;
                case WalletType.Hardware:
                    await WalletProvider.Init();
                    break;
                case WalletType.Web:
                    await WalletProvider.Init();
                    await WebWalletCheckingState();
                    break;
            }

            if (accessTokenExpired)
            {
                WalletManagerService.Logout();
            }
        }

        public async Task ConnectToExtensionWallet()
        {
            WalletProvider = new ExtensionWalletProvider(JsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                await WalletProvider.Init();
                var accountInfo = await WalletProvider.Login(_authToken);
                await ValidateWalletConnection(JsonWrapper.Deserialize<AccountToken>(accountInfo));
            }
            catch { }
        }

        public async Task ConnectToXPortalWallet()
        {
            WalletProvider = new XPortalWalletProvider(JsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                await WalletProvider.Init();
                await WalletProvider.Login(_authToken);
            }
            catch { }
        }

        public async Task ConnectToWebWallet()
        {
            WalletProvider = new WebWalletProvider(JsRuntime);
            try
            {
                _authToken = await _nativeAuthService.GenerateToken();

                await WalletProvider.Init();
                await WalletProvider.Login(_authToken);
            }
            catch { }
        }

        public async Task ConnectToHardwareWallet(string authToken)
        {
            WalletProvider = new HardwareWalletProvider(JsRuntime);
            _authToken = authToken;
            //Init is previously called from modal
            var accountInfo = await WalletProvider.Login(_authToken);
            await ValidateWalletConnection(JsonWrapper.Deserialize<AccountToken>(accountInfo));
        }

        public async Task ConnectToWebView(string accessToken)
        {
            WalletProvider = new WebViewProvider(JsRuntime);
            var parts = accessToken.Split('.');
            _authToken = Encoding.UTF8.GetString(Convert.FromBase64String(Pad(parts[1])));
            var accountToken = new AccountToken()
            {
                Address = Encoding.UTF8.GetString(Convert.FromBase64String(Pad(parts[0]))),
                Signature = parts[2]
            };
            await ValidateWalletConnection(accountToken);
        }

        private static string Pad(string str)
        {
            if (str.Length % 4 == 0) return str;
            else if (str.Length % 4 == 2) return str += "==";
            else if (str.Length % 4 == 3) return str += "=";
            return str;
        }

        public bool IsConnected()
        {
            if (WalletProvider is null) return false;

            try
            {
                var accessToken = _localStorage.GetItemAsString(ACCESS_TOKEN);
                return !string.IsNullOrEmpty(accessToken);
            }
            catch { return false; }
        }

        public async Task Logout()
        {
            if (WalletProvider is null) return;


            await WalletProvider.Logout();
            WalletDisconnected();
        }

        public string GetAddress()
        {
            return _localStorage.GetItem<AccountToken>(ACCOUNT_TOKEN).Address;
        }

        public async Task<bool?> SignMessage(SignableMessage signableMessage)
        {
            if (WalletProvider is null) return null;

            var signedMessage = await WalletProvider.SignMessage(signableMessage.Message);
            if (_localStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Web) return null;

            if (signedMessage == "canceled")
            {
                return false;
            }

            var messageSignature = JsonWrapper.Deserialize<MessageSignature>(signedMessage);
            var message = new SignableMessage()
            {
                Address = Address.FromBech32(GetAddress()),
                Message = Encoding.Default.GetString(Convert.FromHexString(messageSignature.Message[2..])),
                Signature = messageSignature.Signature[2..]
            };

            return await Http.PostAsync<bool>("api/wallet/verify", message);
        }

        public async Task<string?> SignAndSendTransaction(TransactionRequest transactionRequest, string title = "Transaction")
        {
            if (WalletProvider is null) return null;

            if (_localStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Web) _sessionStorage.SetItemAsString(TX_TITLE, title);

            var signedTransaction = await WalletProvider.SignTransaction(transactionRequest);
            if (_localStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Web) return "";

            if (signedTransaction == "canceled")
            {
                await WalletProvider.TransactionIsCanceled();
                _postTxSendService.Clear();
                return null;
            }

            return await SendTransaction(signedTransaction, title);
        }

        private async Task<string?> SendTransaction(string signedTransaction, string title = "Transaction")
        {
            try
            {
                var transaction = JsonWrapper.Deserialize<TransactionRequestDto>(signedTransaction);
                return await SendTransaction(transaction, title);
            }
            catch
            {
                _postTxSendService.Clear();
                return null;
            }
        }

        private async Task<string> SendTransaction(TransactionRequestDto transaction, string title)
        {
            var response = await Provider.SendTransaction(transaction);

            await RunPostTxSendProcess();

            TransactionsContainer.NewTransaction(title, response.TxHash);
            return response.TxHash;
        }

        public async Task<string[]?> SignAndSendTransactions(TransactionRequest[] transactionsRequest, string title = "Transactions")
        {
            if (WalletProvider is null) return null;

            if (_localStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Web) _sessionStorage.SetItemAsString(TX_TITLE, title);

            var signedTransactions = await WalletProvider.SignTransactions(transactionsRequest);
            if (_localStorage.GetItem<WalletType>(WALLET_TYPE) == WalletType.Web) return Array.Empty<string>();

            if (signedTransactions == "canceled")
            {
                await WalletProvider.TransactionIsCanceled();
                _postTxSendService.Clear();
                return null;
            }

            return await SendTransactions(signedTransactions, title);
        }

        private async Task<string[]?> SendTransactions(string signedTransactions, string title = "Transactions")
        {
            try
            {
                var transactions = JsonWrapper.Deserialize<TransactionRequestDto[]>(signedTransactions);
                return await SendTransactions(transactions, title);
            }
            catch
            {
                _postTxSendService.Clear();
                return null;
            }
        }

        private async Task<string[]> SendTransactions(TransactionRequestDto[] transactions, string title)
        {
            var response = await Provider.SendTransactions(transactions);

            await RunPostTxSendProcess();

            TransactionsContainer.NewTransaction(title, response.TxsHashes.Select(tx => tx.Value).ToArray());
            return response.TxsHashes.Select(tx => tx.Value).ToArray();
        }

        public async Task CancelAction()
        {
            if (WalletProvider is null) return;

            await WalletProvider.CancelAction();
        }

        public void PreparePostTxSendProcess(PostTxSendProcess process, object @object)
        {
            if (WalletProvider is null) return;

            _sessionStorage.SetItem(POST_PROCESS, process);
            _sessionStorage.SetItem(POST_PROCESS_OBJECT, @object);
        }

        //After TX is sent successfully (tx is sent but you don't know if it will be success or fail at this stage), you can use this function to do a Post (Send) Process like a request to Mx.Blazor.DApp.Sever API

        //You can use the events TransactionsContainer.TxExecuted or TransactionsContainer.HashesExecuted to do a process after the TX is processed and you know the end state (success/fail/etc.)
        //Keep in mind that the TxExecuted/HashesExecuted events will not be triggered if the user will close the website before the tx is processed, so if you want to do an UPDATE in database it might not run...
        public async Task RunPostTxSendProcess()
        {
            await _postTxSendService.Run();
        }

        public async Task WebWalletCheckingState()
        {
            switch (_localStorage.GetItem<WebWalletState>(WEB_WALLET_STATE))
            {
                case WebWalletState.None:
                    break;

                case WebWalletState.LoggingIn:
                    var accountToken = NavigationManager.Uri.GetAccountTokenFromUrl();
                    _authToken = _sessionStorage.GetItemAsString(AUTH_TOKEN);
                    await ValidateWalletConnection(accountToken);
                    _sessionStorage.RemoveItem(AUTH_TOKEN);

                    NavigationManager.NavigateTo(NavigationManager.Uri.GetUrlWithoutParameters());

                    _localStorage.SetItem(WEB_WALLET_STATE, WebWalletState.None);
                    break;

                case WebWalletState.WaitingForSig:
                    var signature = NavigationManager.Uri.GetSignatureFromUrl();
                    var message = _sessionStorage.GetItemAsString(SIG_MESSAGE);
                    _sessionStorage.RemoveItem(SIG_MESSAGE);

                    NavigationManager.NavigateTo(NavigationManager.Uri.GetUrlWithoutParameters());

                    try
                    {
                        var messageSignature = new MessageSignature(message, signature);
                        var signableMessage = new SignableMessage()
                        {
                            Address = Address.FromBech32(GetAddress()),
                            Message = messageSignature.Message,
                            Signature = messageSignature.Signature
                        };

                        Console.WriteLine(await Http.PostAsync<bool>("api/wallet/verify", signableMessage));
                    }
                    catch { }
                    finally
                    {
                        _localStorage.SetItem(WEB_WALLET_STATE, WebWalletState.None);
                    }
                    break;

                case WebWalletState.WaitingForTx:
                    var signedRequests = NavigationManager.Uri.GetTransactionsFromUrl();

                    NavigationManager.NavigateTo(NavigationManager.Uri.GetUrlWithoutParameters());

                    try
                    {
                        if (signedRequests == "canceled")
                        {
                            await WalletProvider.TransactionIsCanceled();
                        }
                        else
                        {
                            await SendTransactions(signedRequests, _sessionStorage.GetItemAsString(TX_TITLE));

                            await RunPostTxSendProcess();
                        }
                    }
                    catch { }
                    finally
                    {
                        _sessionStorage.RemoveItem(TX_TITLE);
                        _localStorage.SetItem(WEB_WALLET_STATE, WebWalletState.None);
                    }
                    break;
            }
        }
    }
}
