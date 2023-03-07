namespace Mx.Blazor.DApp.Client.Models
{
    public class MessageSignature
    {
        public string Message { get; set; }
        public string Signature { get; set; }

        public MessageSignature(string message, string signature)
        {
            Message = message;
            Signature = signature;
        }
    }
}
