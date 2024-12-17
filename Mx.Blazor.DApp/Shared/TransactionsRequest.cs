using Mx.NET.SDK.Domain.Data.Network;

namespace Mx.Blazor.DApp.Shared;

public class TransactionsRequest
{
    public string InitiatorAddress { get; init; } = string.Empty;
    public string ExecutorAddress { get; init; } = string.Empty;
    public string ContractAddress { get; init; } = string.Empty;
    public int NrOfTransactions { get; init; }
    public DateTime ScheduledTime { get; init; } = DateTime.UtcNow;
}
