using System;
using System.Diagnostics.CodeAnalysis;

namespace DocumentsApi.V1.Helpers
{
    [SuppressMessage("ReSharper", "CA1055")]
    [SuppressMessage("ReSharper", "CA1054")]
    public static class Base64UrlHelpers
    {
        public static string EncodeToBase64Url(string s)
        {
            var byteArrayId = StringToByteArray(s);
            string base64Url = Convert.ToBase64String(byteArrayId);
            base64Url = base64Url.Split('=')[0]; // Remove any trailing '='s
            base64Url = base64Url.Replace('+', '-'); // 62nd char of encoding
            base64Url = base64Url.Replace('/', '_'); // 63rd char of encoding
            return base64Url;
        }

        public static string DecodeFromBase64Url(string base64UrlEncoded)
        {
            base64UrlEncoded = base64UrlEncoded.Replace('-', '+'); // 62nd char of encoding
            base64UrlEncoded = base64UrlEncoded.Replace('_', '/'); // 63rd char of encoding
            switch (base64UrlEncoded.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: base64UrlEncoded += "=="; break; // Two pad chars
                case 3: base64UrlEncoded += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var decodedByteArray = Convert.FromBase64String(base64UrlEncoded);
            var decodedString = ByteArrayToString(decodedByteArray);
            return decodedString;
        }

        public static byte[] StringToByteArray(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }

        public static string ByteArrayToString(byte[] byteArray)
        {
            return System.Text.Encoding.Default.GetString(byteArray);
        }
    }
}
