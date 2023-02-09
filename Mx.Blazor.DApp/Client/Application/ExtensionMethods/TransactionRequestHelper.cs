using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Domain;
using Mx.NET.SDK.Provider.Dtos.API.Transactions;
using System.Text;
using System.Web;

namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class TransactionRequestHelper
    {
        public static TransactionRequestDto GetTransactionRequestDecoded(this TransactionRequest transactionRequest)
        {
            var transaction = transactionRequest.GetTransactionRequest();
            transaction.Data = transaction.Data is null ? "" : DataCoder.DecodeData(transaction.Data);

            return transaction;
        }

        public static TransactionRequestDto[] GetTransactionsRequestDecoded(this TransactionRequest[] transactionsRequest)
        {
            var transactions = new List<TransactionRequestDto>();
            foreach (var transactionRequest in transactionsRequest)
            {
                var transaction = transactionRequest.GetTransactionRequest();
                transaction.Data = transaction.Data is null ? "" : DataCoder.DecodeData(transaction.Data);
                transactions.Add(transaction);
            }

            return transactions.ToArray();
        }

        public static string GetTransactionsFromUrl(this string urlString)
        {
            int idx = urlString.IndexOf('?');
            string query = idx >= 0 ? urlString[idx..] : "";
            var args = HttpUtility.ParseQueryString(query);

            if (args["status"] == "cancelled" || args.Count == 0)
                return "canceled";

            if (args["status"] != null && args["status"] != "transactionsSigned")
                return "canceled";

            try
            {
                var transactions = new List<TransactionRequestDto>();
                for (var i = 0; i < args.Count / 10; i++)
                    transactions.Add(new TransactionRequestDto()
                    {
                        Nonce = ulong.Parse(args[$"nonce[{i}]"]),
                        Value = args[$"value[{i}]"],
                        Receiver = args[$"receiver[{i}]"],
                        Sender = args[$"sender[{i}]"],
                        GasPrice = long.Parse(args[$"gasPrice[{i}]"]),
                        GasLimit = long.Parse(args[$"gasLimit[{i}]"]),
                        Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(args[$"data[{i}]"])),
                        ChainID = args[$"chainId[{i}]"],
                        Version = int.Parse(args[$"version[{i}]"]),
                        Signature = args[$"signature[{i}]"],
                    });

                return JsonWrapper.Serialize(transactions);
            }
            catch
            {
                return "canceled";
            }
        }
    }
}
