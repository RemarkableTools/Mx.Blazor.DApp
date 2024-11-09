namespace Mx.Blazor.DApp.Client.Application.ExtensionMethods
{
    public static class UrlHelper
    {
        public static string GetHost(this string urlString)
        {
            var uri = new Uri(urlString);
            return $"{uri.Scheme}{Uri.SchemeDelimiter}{uri.Host}";
        }

        public static string GetUrlWithoutParameters(this string urlString)
        {
            var idx = urlString.IndexOf('?');
            return idx == -1 ? urlString : urlString[..idx];
        }
    }
}
