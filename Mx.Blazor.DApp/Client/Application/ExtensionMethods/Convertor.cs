namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class Convertor
    {
        public static long ToTimestamp(this DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }
    }
}
