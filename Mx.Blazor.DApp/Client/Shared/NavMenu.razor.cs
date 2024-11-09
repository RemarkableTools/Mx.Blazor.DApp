using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Services.Wallet;

namespace Mx.Blazor.DApp.Client.Shared
{
    public partial class NavMenu
    {
        [CascadingParameter]
        private bool WalletConnected { get; set; }

        private bool _collapseNavMenu = true;
        private string? NavMenuCssClass => _collapseNavMenu ? "collapse" : null;
        private void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;
        }

        private static void Disconnect()
        {
            WalletManagerService.Logout();
        }
    }
}
