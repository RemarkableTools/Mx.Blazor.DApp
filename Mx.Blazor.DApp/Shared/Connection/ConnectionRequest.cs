namespace Mx.Blazor.DApp.Shared.Connection
{
    public class ConnectionRequest
    {
        public AccountToken AccountToken { get; set; } = new();
        public string? AuthToken { get; set; }

        public bool IsValid()
            => !string.IsNullOrEmpty(AuthToken) && AccountToken.IsValid();
    }
}
