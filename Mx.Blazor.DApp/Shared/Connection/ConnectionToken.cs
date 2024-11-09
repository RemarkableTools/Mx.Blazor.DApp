namespace Mx.Blazor.DApp.Shared.Connection
{
    public class ConnectionToken(
        AccountToken accountToken,
        string accessToken)
    {
        public AccountToken AccountToken { get; } = accountToken;
        public string AccessToken { get; } = accessToken;
    }
}
