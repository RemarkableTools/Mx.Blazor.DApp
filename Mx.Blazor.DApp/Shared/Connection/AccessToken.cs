namespace Mx.Blazor.DApp.Shared.Connection
{
    public class AccessToken
    {
        public string Value { get; set; }
        public AccessToken(string value)
        {
            Value = value;
        }
    }
}
