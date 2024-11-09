using Microsoft.AspNetCore.Components;

namespace Mx.Blazor.DApp.Client.Shared.Components.Login
{
    public partial class LoginBtn
    {
        [Parameter]
        public string Icon { get; set; } = string.Empty;

        [Parameter]
        public string Name { get; set; } = string.Empty;

        [Parameter]
        public EventCallback ConnectEvent { get; set; }
    }
}
