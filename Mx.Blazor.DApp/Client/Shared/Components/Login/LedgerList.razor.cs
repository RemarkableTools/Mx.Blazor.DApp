using Microsoft.AspNetCore.Components;

namespace Mx.Blazor.DApp.Client.Shared.Components.Login
{
    public partial class LedgerList
    {
        [Parameter]
        public List<string> Addresses { get; set; } = default!;

        [Parameter]
        public EventCallback<int> CheckChanged { get; set; }
    }
}
