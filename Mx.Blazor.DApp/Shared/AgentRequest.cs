namespace Mx.Blazor.DApp.Shared;

public class AgentRequest
{
    public string InputMessage {get; set;}
    public string ChainId { get; set; }
    public string Sender { get; set; }
    public string ContractAddress { get; set; }
    public string ServiceAddress { get; set; }
}
