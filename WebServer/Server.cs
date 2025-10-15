using System;
using System.Collections.Generic;
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
            string websitePath = Assembly.GetExecutingAssembly().Location;
            websitePath = websitePath.LeftOfRightmostOf("\\").LeftOfRightmostOf("\\").LeftOfRightmostOf("\\") + "\\Website";
            return websitePath;
        }

        public static void Start()
        {
            string websitePath = GetWebsitePath();
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
            HttpListenerContext context = await listener.GetContextAsync();
            sem.Release();
            Log(context.Request);

            HttpListenerRequest request = context.Request;
            string path = request.RawUrl.LeftOf("?");
            string verb = request.HttpMethod;
            string parms = request.RawUrl.RightOf("?");
            Dictionary<string, string> kvParams = GetKeyValues(parms);

            ResponsePacket resp = router.Route(verb, path, kvParams);
            if (resp != null)
            {
                Respond(context.Response, resp);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
            }
        }

        private static Dictionary<string, string> GetKeyValues(string parms)
        {
            var kvParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(parms))
            {
                foreach (var pair in parms.Split('&'))
                {
                    var keyValue = pair.Split('=');
                    if (keyValue.Length == 2)
                        kvParams[keyValue[0]] = keyValue[1];
                }
            }
            return kvParams;
        }

        public static void Log(HttpListenerRequest request)
        {
            Console.WriteLine($"{request.RemoteEndPoint} {request.HttpMethod} {request.Url.AbsolutePath}");
        }

        private static void Respond(HttpListenerResponse response, ResponsePacket resp)
        {
            response.ContentType = resp.ContentType;
            response.ContentLength64 = resp.Data.Length;
            response.OutputStream.Write(resp.Data, 0, resp.Data.Length);
            response.ContentEncoding = resp.Encoding;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.OutputStream.Close();
        }
    }
}