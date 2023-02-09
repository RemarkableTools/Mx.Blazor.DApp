using Microsoft.AspNetCore.Components;

namespace Mx.Blazor.DApp.Client.Shared.Components.Login
{
    public partial class LoginBtn
    {
        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public EventCallback ConnectEvent { get; set; }
    }
}
