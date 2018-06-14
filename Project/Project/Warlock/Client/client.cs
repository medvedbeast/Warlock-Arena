using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Library;

namespace Client
{
    [Serializable]
    class client
    {
        public delegate void RegistryDelegate(bool reg);
        public event RegistryDelegate RegistryMan;
        public event RegistryDelegate RegistryChat;

        public delegate void MessageDelegate(string chat_Name, string message);
        public event MessageDelegate MessageEvent;

        byte[] buf = new byte[1024];
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        IPEndPoint ip = new IPEndPoint(IPAddress.Parse("10.6.0.139"), 2014);
        string Login = "";
        public client()
        {
            
        }
        void ReceiveCallback(IAsyncResult res)
        {
            Socket s = res.AsyncState as Socket;
            try
            {
                int count = s.EndReceive(res);
                if (count > 0)
                {
                    Message bmp = Message.GetData(buf);
                    switch (bmp.type)
                    {
                        case MESSAGE_TYPE.SERVER_ANSWER_LOGIN:
                            {
                                if ((bool)bmp.parameters[0] == true && RegistryMan != null)
                                    RegistryMan(true);
                                else if (RegistryMan != null)
                                    RegistryMan(false);
                                break;
                            }
                        case MESSAGE_TYPE.SERVER_ANSWER_MESSAGE:
                            {
                                if (MessageEvent != null)
                                    MessageEvent(bmp.parameters[0].ToString(), bmp.parameters[1].ToString());
                                break;
                            }
                        case MESSAGE_TYPE.SERVER_ANSWER_JOIN_CHAT_CHANNEL:
                            {
                                if ((bool)bmp.parameters[0] == true && RegistryChat != null)
                                    RegistryChat(true);
                                else if (RegistryChat != null)
                                    RegistryChat(false);
                                break;
                            }
                        case MESSAGE_TYPE.SERVER_ANSWER_GET_ONLINE_LIST:
                            {
                                break;
                            }
                        case MESSAGE_TYPE.SERVER_ANSWER_GET_CHAT_CHANNELS:
                            {
                                break;
                            }
                        case MESSAGE_TYPE.SERVER_ANSWER_CREATE_CHAT_CHANNEL:
                            {
                                break;
                            }
                    }
                    s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Server off");
            }
        }
        //Команды для сервера
        public void Connect(string Login)
        {
            try
            {
                s.Connect(ip);
                s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                this.Login = Login;
                Message mes = new Message(MESSAGE_TYPE.REQUEST_LOGIN, Login, null);
                s.Send(Message.GetBytes(mes));
            }
            catch (Exception)
            {
            }
        }
        public void Get_Online_List()
        {
            Message mes = new Message(MESSAGE_TYPE.REQUEST_GET_ONLINE_LIST, Login, null);
            s.Send(Message.GetBytes(mes));
        }
        public void Create_Chat_Channel(string name, string pass, int count)
        {
            Message mes = new Message(MESSAGE_TYPE.REQUEST_CREATE_CHAT_CHANNEL, Login, new object[] { name, pass, count });
            s.Send(Message.GetBytes(mes));
        }
        public void Join_Chat_Channel(string name, string pass)
        {
            Message mes = new Message(MESSAGE_TYPE.REQUEST_JOIN_CHAT_CHANNEL, Login, new object[] { name, pass });
            s.Send(Message.GetBytes(mes));
        }
        public void Get_Chat_Channels()
        {
            Message mes = new Message(MESSAGE_TYPE.REQUEST_GET_CHAT_CHANNELS, Login, null);
            s.Send(Message.GetBytes(mes));
        }
        public void New_Message(string message, string name_channel)
        {
            Message mes = new Message(MESSAGE_TYPE.REQUEST_MESSAGE, Login, new object[]{message, name_channel});
            s.Send(Message.GetBytes(mes));
        }
    }
}
