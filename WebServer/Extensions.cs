using System;
using System.Text;

namespace WebServer
{
    public enum ServerError
    {
        OK,
        ExpiredSession,
        NotAuthorized,
        FileNotFound,
        PageNotFound,
        ServerError,
        UnknownType
    }
    //public class Route
    //{
    //    public string Verb { get; set; }
    //    public string Path { get; set; }
    //    public Func<Dictionary<string, string>, string> Action { get; set; }
    //}

    public class Route
    {
        public string Verb { get; set; }
        public string Path { get; set; }
        public RouteHandler Handler { get; set; } // Updated to use RouteHandler
    }
    public class ResponsePacket
    {
        public string Redirect { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public Encoding Encoding { get; set; }
    }

    internal class ExtensionInfo
    {
        public string ContentType { get; set; }
        public Func<string, string, ExtensionInfo, ResponsePacket> Loader { get; set; }
    }

    public static class StringExtensions
    {
        public static string LeftOf(this string input, string delimiter)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(delimiter))
                return input;
            int index = input.IndexOf(delimiter);
            return index == -1 ? input : input.Substring(0, index);
        }

        public static string RightOf(this string input, string delimiter)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(delimiter))
                return input;
            int index = input.IndexOf(delimiter);
            return index == -1 ? string.Empty : input.Substring(index + delimiter.Length);
        }

        public static string LeftOfRightmostOf(this string input, string delimiter)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(delimiter))
                return input;
            int index = input.LastIndexOf(delimiter);
            return index == -1 ? input : input.Substring(0, index);
        }
    }
}