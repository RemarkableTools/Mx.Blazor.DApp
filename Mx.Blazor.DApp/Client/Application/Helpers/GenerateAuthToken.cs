using System.Text;

namespace Mx.Blazor.DApp.Client.Application.Helpers
{
    public static class GenerateAuthToken
    {
        public static string Random()
        {
            const string src = "abcdefghijklmnopqrstuvwxyz0123456789";
            int length = 32;
            StringBuilder sb = new();
            Random rand = new();
            for (var i = 0; i < length; i++)
            {
                var c = src[rand.Next(0, src.Length)];
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
