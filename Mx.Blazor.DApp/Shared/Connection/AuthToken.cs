using Mx.NET.SDK.Core.Domain.Helper;
using System.Text;

namespace Mx.Blazor.DApp.Shared.Connection
{
    public class AuthToken
    {
        public string Host { get; set; }
        public string BlockHash { get; set; }
        public int TimeToLive { get; set; }
        public ExtraInfo ExtraInfo { get; set; }

        public AuthToken(string authToken)
        {
            var parts = authToken.Split('.');
            Host = Unescape(Encoding.UTF8.GetString(Convert.FromBase64String(Pad(parts[0]))));
            BlockHash = Unescape(Encoding.UTF8.GetString(Convert.FromBase64String(Pad(parts[1]))));
            TimeToLive = int.Parse(parts[2]);
            ExtraInfo = JsonWrapper.Deserialize<ExtraInfo>(Unescape(Encoding.UTF8.GetString(Convert.FromBase64String(Pad(parts[3])))));
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Host)) return false;

            if (string.IsNullOrEmpty(BlockHash)) return false;

            if (TimeToLive <= 0) return false;

            if (!ExtraInfo.IsValid()) return false;

            var currentTimestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            if (TimeToLive + ExtraInfo.Timestamp < currentTimestamp) return false;

            return true;
        }

        private static string Unescape(string str)
            => str.Replace('-', '+').Replace('+', '/');

        private static string Pad(string str)
        {
            if (str.Length % 4 == 0) return str;
            else if (str.Length % 4 == 2) return str += "==";
            else if (str.Length % 4 == 3) return str += "=";
            return str;
        }
    }

    public class ExtraInfo
    {
        public long Timestamp { get; set; }

        public ExtraInfo(long timestamp)
        {
            Timestamp = timestamp;
        }

        public bool IsValid()
            => Timestamp > 0;
    }


}
