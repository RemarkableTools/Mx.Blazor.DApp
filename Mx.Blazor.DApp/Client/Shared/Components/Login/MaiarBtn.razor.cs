using Microsoft.AspNetCore.Components;

namespace Mx.Blazor.DApp.Client.Shared.Components.Login
{
    public partial class MaiarBtn
    {
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public EventCallback ConnectEvent { get; set; }

        [Parameter]
        public EventCallback ConnectV2Event { get; set; }
    }
}
