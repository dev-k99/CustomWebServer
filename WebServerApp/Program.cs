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
            Server.onRequest = (session, context) =>
            {
                session.Authorized = true; // Spoof authentication for testing
                session.UpdateLastConnectionTime();
            };

            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/redirect", Handler = new AnonymousRouteHandler(RedirectMe) });
            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/auth", Handler = new AuthenticatedRouteHandler(RedirectMe) });
            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/auth-exp", Handler = new AuthenticatedExpirableRouteHandler(RedirectMe) });

            Server.Start();
        }

        public static string RedirectMe(Session session, Dictionary<string, string> parms)
        {
            return "/demo/clicked";
        }
    }
}
