using Mx.Blazor.DApp.Shared.Connection;
using System.Web;

namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class UrlHelper
    {
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
                Address = args["address"] ?? "",
                Signature = args["signature"]
            };
        }

        public static string GetUrlWithoutParameters(this string urlString)
        {
            int idx = urlString.IndexOf('?');
            if (idx == -1)
                return urlString;
            else
                return urlString[..idx];
        }
    }
}
