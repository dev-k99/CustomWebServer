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
            Server.Start();
            Console.ReadLine();
        }
    }
}
