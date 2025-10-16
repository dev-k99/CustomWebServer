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
            Server.AddRoute(new Route { Verb = Server.POST, Path = "/demo/redirect", Action = RedirectMe });
            Server.Start();
        }

        public static string RedirectMe(Dictionary<string, string> parms)
        {
            return "/demo/clicked";
        }
    }
}
