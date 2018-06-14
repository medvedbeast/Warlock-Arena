using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                client cl = new client();
                cl.Connect(Console.ReadLine());
                cl.RegistryMan += new client.RegistryDelegate(cl_Registry);
                cl.MessageEvent += new client.MessageDelegate(cl_MessageEvent);
                cl.RegistryChat += new client.RegistryDelegate(cl_RegistryChat);
                cl.New_Message("text", "All");
                Console.ReadLine();
                cl.Join_Chat_Channel("MyChat", "123");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void cl_RegistryChat(bool reg)
        {
            if(reg)
                Console.WriteLine("Chat registry");
            else
                Console.WriteLine("Chat not registry");
        }

        static void cl_MessageEvent(string chat_Name, string message)
        {
            Console.WriteLine(chat_Name + ": " + message);
        }

        static void cl_Registry(bool reg)
        {
            if (reg)
                Console.WriteLine("Man registry");
            else
                Console.WriteLine("Man not registry");
        }
    }
}
