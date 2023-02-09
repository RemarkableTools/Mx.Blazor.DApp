using Blazored.LocalStorage;
using static Mx.Blazor.DApp.Client.Application.Constants.BrowserStorage;

namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class Extensions
    {
        public static void RemoveAllWcItems(this ISyncLocalStorageService localStorage)
        {
            localStorage.RemoveItem(WALLET_CONNECT_DEF_STORAGE);
            localStorage.RemoveItems(localStorage.Keys().Where(k => k.StartsWith("wc@2")));
        }
    }
}
