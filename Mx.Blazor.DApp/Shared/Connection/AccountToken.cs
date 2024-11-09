namespace Mx.Blazor.DApp.Shared.Connection
{
    public class AccountToken
    {
        public string Address { get; init; } = "";
        public string? Signature { get; init; }

        public bool IsValid()
            => Address != "" && !string.IsNullOrEmpty(Signature);
    }
}
