using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Application.Constants;
using Mx.NET.SDK.Configuration;
using Mx.NET.SDK.Provider;

namespace Mx.Blazor.DApp.Client.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        private bool collapseNavMenu = true;
        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        public async void Disconnect()
        {
            await WalletProvider.Logout();
        }
    }
}
