using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            server s = new server();
            Console.ReadKey();
        }
    }
}
