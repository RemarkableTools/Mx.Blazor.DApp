using Mx.Blazor.DApp.Shared.Connection;
using Mx.NET.SDK.Core.Domain.Helper;
using Mx.NET.SDK.Provider.Dtos.Common.Transactions;
using System.Text;
using System.Web;

namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class UrlHelper
    {
        public static string GetHost(this string urlString)
        {
            return new Uri(urlString).Host;
        }

        public static string GetUrlWithoutParameters(this string urlString)
        {
            int idx = urlString.IndexOf('?');
            if (idx == -1)
                return urlString;
            else
                return urlString[..idx];
        }

        public static AccountToken GetAccountTokenFromUrl(this string urlString)
        {
            int idx = urlString.IndexOf('?');
            string query;
            if (idx == -1)
                return new AccountToken();

            query = urlString.Substring(idx);
            var args = HttpUtility.ParseQueryString(query);
            if (args["address"] == null) return new AccountToken();

            return new AccountToken()
            {
                Address = args["address"] ?? string.Empty,
                Signature = args["signature"]
            };
        }

        public static string GetSignatureFromUrl(this string urlString)
        {
            int idx = urlString.IndexOf('?');
            string query = idx >= 0 ? urlString[idx..] : string.Empty;
            var args = HttpUtility.ParseQueryString(query);

            if (args.Count == 0) return string.Empty;
            if (args["status"] == null || args["status"] != "signed") return string.Empty;

            return args["signature"] ?? string.Empty;
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
                var len = args.AllKeys.Where(k => k.StartsWith("nonce[")).Count();
                var transactions = new List<TransactionRequestDto>();
                for (var i = 0; i < len; i++)
                    transactions.Add(new TransactionRequestDto()
                    {
                        Nonce = ulong.Parse(args[$"nonce[{i}]"]),
                        Value = args[$"value[{i}]"],
                        Receiver = args[$"receiver[{i}]"],
                        Sender = args[$"sender[{i}]"],
                        GasPrice = long.Parse(args[$"gasPrice[{i}]"]),
                        GasLimit = long.Parse(args[$"gasLimit[{i}]"]),
                        Data = IsBase64String(args[$"data[{i}]"]) ? args[$"data[{i}]"] : DataCoder.EncodeData(args[$"data[{i}]"]),
                        ChainID = args[$"chainId[{i}]"],
                        Version = int.Parse(args[$"version[{i}]"]),
                        Options = args[$"options[{i}]"] is null ? null : int.Parse(args[$"options[{i}]"]),
                        Guardian = args[$"guardian[{i}]"],
                        Signature = args[$"signature[{i}]"],
                        GuardianSignature = args[$"guardianSignature[{i}]"]
                    });

                return JsonWrapper.Serialize(transactions);
            }
            catch
            {
                return "canceled";
            }
        }

        private static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }
    }
}
