namespace Mx.Blazor.DApp.Client.Models
{
    public class StorageTx
    {
        public string Title { get; set; }
        public List<TransactionData> Txs { get; set; }
        public long Timestamp { get; set; }

        public DateTime ToDate()
        {
            return DateTime.UnixEpoch.AddSeconds(Timestamp).ToUniversalTime();
        }

        public StorageTx(string title, List<TransactionData> txs, long timestamp)
        {
            Title = title;
            Txs = txs;
            Timestamp = timestamp;
        }
    }

    public class TransactionData
    {
        public string Hash { get; set; }
        public string Status { get; set; }

        public TransactionData(string hash, string status = "pending")
        {
            Hash = hash;
            Status = status;
        }
    }
}
