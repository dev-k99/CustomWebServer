using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebServer
{
    public class Router
    {
        public string WebsitePath { get; set; }

        private readonly Dictionary<string, ExtensionInfo> extFolderMap;

        public Router()
        {
            extFolderMap = new Dictionary<string, ExtensionInfo>
            {
                { "ico", new ExtensionInfo { Loader = ImageLoader, ContentType = "image/ico" } },
                { "png", new ExtensionInfo { Loader = ImageLoader, ContentType = "image/png" } },
                { "jpg", new ExtensionInfo { Loader = ImageLoader, ContentType = "image/jpg" } },
                { "gif", new ExtensionInfo { Loader = ImageLoader, ContentType = "image/gif" } },
                { "bmp", new ExtensionInfo { Loader = ImageLoader, ContentType = "image/bmp" } },
                { "html", new ExtensionInfo { Loader = PageLoader, ContentType = "text/html" } },
                { "css", new ExtensionInfo { Loader = FileLoader, ContentType = "text/css" } },
                { "js", new ExtensionInfo { Loader = FileLoader, ContentType = "text/javascript" } },
                { "", new ExtensionInfo { Loader = PageLoader, ContentType = "text/html" } }
            };
        }

        public ResponsePacket Route(string verb, string path, Dictionary<string, string> kvParams)
        {
            string ext = path.RightOf(".");
            ResponsePacket ret = null;

            if (extFolderMap.TryGetValue(ext, out var extInfo))
            {
                string fullPath = Path.Combine(WebsitePath, path.TrimStart('/').Replace("/", "\\"));
                ret = extInfo.Loader(fullPath, ext, extInfo);
            }

            return ret;
        }

        private ResponsePacket ImageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            try
            {
                using var fStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                using var br = new BinaryReader(fStream);
                return new ResponsePacket { Data = br.ReadBytes((int)fStream.Length), ContentType = extInfo.ContentType };
            }
            catch
            {
                return null; // Will be handled as 404 later
            }
        }

        private ResponsePacket FileLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            try
            {
                string text = File.ReadAllText(fullPath);
                return new ResponsePacket { Data = Encoding.UTF8.GetBytes(text), ContentType = extInfo.ContentType, Encoding = Encoding.UTF8 };
            }
            catch
            {
                return null; // Will be handled as 404 later
            }
        }

        private ResponsePacket PageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            if (fullPath == WebsitePath) // Root request (e.g., foo.com)
            {
                return Route("GET", "/index.html", null);
            }

            if (string.IsNullOrEmpty(ext))
            {
                fullPath += ".html";
            }

            // Inject "Pages" folder for HTML files
            fullPath = Path.Combine(WebsitePath, "Pages", fullPath.RightOf(WebsitePath).TrimStart('\\'));
            return FileLoader(fullPath, ext, extInfo);
        }
    }
}