using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServer
{
    public static class Server
    {
        private static HttpListener listener;
        private static Router router = new Router();
        public static int maxSimultaneousConnections = 20;
        private static Semaphore sem = new Semaphore(maxSimultaneousConnections, maxSimultaneousConnections);
        public static Func<ServerError, string> onError;
        public const string POST = "post"; // Add POST constant

        private static List<IPAddress> GetLocalHostIPs()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
        }

        private static HttpListener InitializeListener(List<IPAddress> localhostIPs)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");

            localhostIPs.ForEach(ip =>
            {
                Console.WriteLine("Listening on IP " + "http://" + ip.ToString() + ":8080/");
                listener.Prefixes.Add("http://" + ip.ToString() + ":8080/");
            });

            return listener;
        }

        public static string GetWebsitePath()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string websitePath = Path.Combine(basePath, "..", "..", "..", "Website");
            return Path.GetFullPath(websitePath);
        }

        public static void Start()
        {
            string websitePath = GetWebsitePath();
            Console.WriteLine($"Website Path: {websitePath}");
            router.WebsitePath = websitePath;
            List<IPAddress> localHostIPs = GetLocalHostIPs();
            listener = InitializeListener(localHostIPs);
            Start(listener);
        }

        private static void Start(HttpListener listener)
        {
            listener.Start();
            Task.Run(() => RunServer(listener));
        }

        private static async Task RunServer(HttpListener listener)
        {
            while (true)
            {
                sem.WaitOne();
                await StartConnectionListener(listener);
            }
        }

        private static async Task StartConnectionListener(HttpListener listener)
        {
            ResponsePacket resp = null;
            HttpListenerContext context = null;

            try
            {
                Console.WriteLine("Waiting for a connection...");
                context = await listener.GetContextAsync();
                Console.WriteLine("Connection received!");
                sem.Release();
                Log(context.Request);

                HttpListenerRequest request = context.Request;
                string path = request.RawUrl.LeftOf("?");
                string verb = request.HttpMethod;
                string parms = request.RawUrl.RightOf("?");
                Dictionary<string, string> kvParams = GetKeyValues(parms);
                string data = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
                GetKeyValues(data, kvParams);
                Log(kvParams);

                Console.WriteLine($"Routing: {verb} {path} with params {parms}");
                resp = router.Route(verb, path, kvParams);

                if (resp != null && resp.Error != ServerError.OK)
                {
                    resp.Redirect = onError?.Invoke(resp.Error);
                }

                Respond(context.Request, context.Response, resp);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                resp = new ResponsePacket { Redirect = onError?.Invoke(ServerError.ServerError) };
                if (context != null)
                    Respond(context.Request, context.Response, resp);
            }
        }

        public static void Log(HttpListenerRequest request)
        {
            Console.WriteLine($"{request.RemoteEndPoint} {request.HttpMethod} {request.Url.AbsolutePath}");
        }

        public static void Log(Dictionary<string, string> kv)
        {
            kv.ForEach(kvp => Console.WriteLine($"{kvp.Key} : {kvp.Value}"));
        }

        private static Dictionary<string, string> GetKeyValues(string parms)
        {
            var kvParams = new Dictionary<string, string>();
            return GetKeyValues(parms, kvParams);
        }

        private static Dictionary<string, string> GetKeyValues(string data, Dictionary<string, string> kv = null)
        {
            kv.IfNull(() => kv = new Dictionary<string, string>());
            data.If(d => d.Length > 0, (d) => d.Split('&').ForEach(keyValue =>
            {
                var parts = keyValue.Split('=');
                if (parts.Length == 2)
                    kv[parts[0]] = parts[1];
            }));
            return kv;
        }

        private static void Respond(HttpListenerRequest request, HttpListenerResponse response, ResponsePacket resp)
        {
            if (resp == null)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Close();
                return;
            }

            if (!string.IsNullOrEmpty(resp.Redirect))
            {
                response.StatusCode = (int)HttpStatusCode.Redirect;
                response.Redirect("http://" + request.UserHostAddress + resp.Redirect);
            }
            else
            {
                response.ContentType = resp.ContentType;
                response.ContentLength64 = resp.Data?.Length ?? 0;
                if (resp.Data != null)
                    response.OutputStream.Write(resp.Data, 0, resp.Data.Length);
                response.ContentEncoding = resp.Encoding;
                response.StatusCode = (int)HttpStatusCode.OK;
            }

            response.OutputStream.Close();
        }

        public static string ErrorHandler(ServerError error)
        {
            string ret = null;
            switch (error)
            {
                case ServerError.ExpiredSession:
                    ret = "/ErrorPages/expiredSession.html";
                    break;
                case ServerError.FileNotFound:
                    ret = "/ErrorPages/fileNotFound.html";
                    break;
                case ServerError.NotAuthorized:
                    ret = "/ErrorPages/notAuthorized.html";
                    break;
                case ServerError.PageNotFound:
                    ret = "/ErrorPages/pageNotFound.html";
                    break;
                case ServerError.ServerError:
                    ret = "/ErrorPages/serverError.html";
                    break;
                case ServerError.UnknownType:
                    ret = "/ErrorPages/unknownType.html";
                    break;
            }
            return ret;
        }

        public static void AddRoute(Route route)
        {
            router.AddRoute(route);
        }
    }
}