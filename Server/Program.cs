using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace WarlockArena2DServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
            s.GetMessage += new Server.MessageDelegate(s_GetMessage);
            Console.ReadLine();
        }

        static void s_GetMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
