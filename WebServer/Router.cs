using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebServer
{
    public class Router
    {
        public string WebsitePath { get; set; }
        private readonly Dictionary<string, ExtensionInfo> extFolderMap;
        private readonly List<Route> routes = new List<Route>();

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

        public void AddRoute(Route route)
        {
            routes.Add(route);
        }

        public ResponsePacket Route(Session session, string verb, string path, Dictionary<string, string> kvParams)
        {
            string ext = path.RightOf(".");
            ResponsePacket ret = null;
            verb = verb.ToLower();

            if (extFolderMap.TryGetValue(ext, out var extInfo))
            {
                string wpath = path.Substring(1).Replace('/', '\\');
                string fullPath = Path.Combine(WebsitePath, wpath);

                Route route = routes.SingleOrDefault(r => verb == r.Verb.ToLower() && path == r.Path);

                if (route != null)
                {
                    string redirect = route.Handler.Handle(session, kvParams);
                    if (string.IsNullOrEmpty(redirect))
                    {
                        ret = extInfo.Loader(fullPath, ext, extInfo);
                    }
                    else
                    {
                        ret = new ResponsePacket { Redirect = redirect, Error = ServerError.OK };
                    }
                }
                else
                {
                    ret = extInfo.Loader(fullPath, ext, extInfo);
                }
            }
            else
            {
                ret = new ResponsePacket { Error = ServerError.UnknownType };
            }

            return ret;
        }

        private ResponsePacket ImageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            try
            {
                using var fStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                using var br = new BinaryReader(fStream);
                return new ResponsePacket { Data = br.ReadBytes((int)fStream.Length), ContentType = extInfo.ContentType, Error = ServerError.OK };
            }
            catch
            {
                return new ResponsePacket { Error = ServerError.FileNotFound };
            }
        }

        private ResponsePacket FileLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            try
            {
                string text = File.ReadAllText(fullPath);
                return new ResponsePacket { Data = Encoding.UTF8.GetBytes(text), ContentType = extInfo.ContentType, Encoding = Encoding.UTF8, Error = ServerError.OK };
            }
            catch
            {
                return new ResponsePacket { Error = ServerError.FileNotFound };
            }
        }

        private ResponsePacket PageLoader(string fullPath, string ext, ExtensionInfo extInfo)
        {
            try
            {
                if (fullPath == WebsitePath)
                {
                    return Route(null, "GET", "/index.html", null);
                }

                if (string.IsNullOrEmpty(ext))
                {
                    fullPath += ".html";
                }

                fullPath = Path.Combine(WebsitePath, "Pages", fullPath.RightOf(WebsitePath).TrimStart('\\'));
                return FileLoader(fullPath, ext, extInfo);
            }
            catch
            {
                return new ResponsePacket { Error = ServerError.PageNotFound };
            }
        }
    }
}