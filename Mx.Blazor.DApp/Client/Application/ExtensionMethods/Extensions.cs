using Blazored.LocalStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserLocalStorage;

namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class Extensions
    {
        public static void RemoveAllWcItems(this ISyncLocalStorageService localStorage)
        {
            localStorage.RemoveItem(WalletConnectDefStorage);
            localStorage.RemoveItems(localStorage.Keys().Where(k => k.StartsWith("wc@2")));
        }
    }
}
