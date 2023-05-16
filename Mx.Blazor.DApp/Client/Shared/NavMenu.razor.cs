using Microsoft.AspNetCore.Components;
using Mx.Blazor.DApp.Client.Services.Wallet;

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

        public void Disconnect()
        {
            WalletManagerService.Logout();
        }
    }
}
