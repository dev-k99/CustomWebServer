using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServer;

namespace WebServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.onError = Server.ErrorHandler;
            Server.postProcess = Server.DefaultPostProcess;
            Server.onRequest = (session, context) =>
            {
                session.Authorized = true; // Spoof authentication
                session.UpdateLastConnectionTime();
            };

            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/redirect", Handler = new AnonymousRouteHandler(RedirectMe) });
            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/auth", Handler = new AuthenticatedRouteHandler(RedirectMe) });
            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/auth-exp", Handler = new AuthenticatedExpirableRouteHandler(RedirectMe) });
            Server.AddRoute(new Route { Verb = Router.PUT, Path = "/demo/ajax", Handler = new AnonymousRouteHandler(AjaxResponder) });
            Server.AddRoute(new Route { Verb = Router.GET, Path = "/demo/ajax", Handler = new AnonymousRouteHandler(AjaxGetResponder) });

            Server.Start();
        }

        public static ResponsePacket RedirectMe(Session session, Dictionary<string, string> parms)
        {
            return Server.Redirect("/demo/clicked");
        }

        public static ResponsePacket AjaxResponder(Session session, Dictionary<string, string> parms)
        {
            string data = "You said " + (parms.TryGetValue("number", out string number) ? number : "no number");
            return new ResponsePacket { Data = Encoding.UTF8.GetBytes(data), ContentType = "text/plain", Encoding = Encoding.UTF8, Error = ServerError.OK };
        }

        public static ResponsePacket AjaxGetResponder(Session session, Dictionary<string, string> parms)
        {
            string data = "GET response: You said " + (parms.TryGetValue("number", out string number) ? number : "no number");
            return new ResponsePacket { Data = Encoding.UTF8.GetBytes(data), ContentType = "text/plain", Encoding = Encoding.UTF8, Error = ServerError.OK };
        }

        
    }
}
