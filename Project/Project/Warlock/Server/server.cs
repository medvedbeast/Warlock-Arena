using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Library;
using System.Text.RegularExpressions;
namespace Server
{
    class server
    {
        byte[] buf = new byte[1024];
        /// <summary>
        /// Все юзеры
        /// </summary>
        List<User> users = new List<User>();
        /// <summary>
        /// Все каналы для чата
        /// </summary>
        List<Channel> channels = new List<Channel>();
        /// <summary>
        /// Все пати
        /// </summary>
        List<Party> parties = new List<Party>();
        public server()
        {
            channels.Add(new Channel("Admin", "All", "", 1000));
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("10.6.0.139"), 2014);
            s.Bind(ip);
            s.Listen(10);
            s.BeginAccept(AcceptCallback, s);
            Console.WriteLine("Server start");
            ServerLog("Server start");
        }
        void AcceptCallback(IAsyncResult res)
        {
            try
            {
                Socket client = res.AsyncState as Socket;
                Socket s = client.EndAccept(res);
                Console.WriteLine(s.RemoteEndPoint);
                ServerLog("Loginned: " + s.RemoteEndPoint.ToString());
                s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                client.BeginAccept(AcceptCallback, client);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ServerLog(ex.Message);
            }
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
                    Console.WriteLine(bmp.type.ToString());
                    ServerLog(s.RemoteEndPoint.ToString() + ": " + bmp.type.ToString());
                    switch (bmp.type)
                    {
                        case MESSAGE_TYPE.REQUEST_LOGIN:
                            {
                                if (bmp.sender == " " || bmp.sender == "")
                                {
                                    Message mes = new Message(MESSAGE_TYPE.SERVER_ANSWER_LOGIN, null, new object[] { false });
                                    s.Send(Message.GetBytes(mes));
                                    s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                                    return;
                                }
                                if (Get_User(bmp.sender).Login != "")
                                {
                                    Message mes = new Message(MESSAGE_TYPE.SERVER_ANSWER_LOGIN, null, new object[] { false });
                                    s.Send(Message.GetBytes(mes));
                                    s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                                    return;
                                }
                                User us = new User(bmp.sender, s);
                                users.Add(us);
                                channels[0].Add(us);
                                List<string> user = new List<string>();
                                for (int i = 0; i < users.Count; i++)
                                    user.Add(users[i].ToString());
                                Message mess = new Message(MESSAGE_TYPE.SERVER_ANSWER_LOGIN, null, new object[] { true, user, "All" });
                                s.Send(Message.GetBytes(mess));
                                Console.WriteLine("Loginned:" + bmp.sender);
                                UpdateOnlineList();
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_CREATE_CHAT_CHANNEL:
                            {
                                for (int i = 0; i < channels.Count; i++)
                                    if (channels[i].Name == bmp.parameters[0].ToString())
                                    {
                                        s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_CREATE_CHAT_CHANNEL, null, new object[] { false })));
                                        s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                                        return;
                                    }
                                Channel ch = new Channel(bmp.sender, bmp.parameters[0].ToString(), bmp.parameters[1].ToString(), Convert.ToInt32(bmp.parameters[2]));
                                ch.users.Add(Get_User(bmp.sender));
                                channels.Add(ch);
                                UpdateOnlineList();
                                s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_CREATE_CHAT_CHANNEL, null, new object[] { true })));
                                Console.WriteLine(bmp.sender + " create channel: " + bmp.parameters[0].ToString() + " pass: " + bmp.parameters[1].ToString() + " count: " + bmp.parameters[2].ToString());
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_JOIN_CHAT_CHANNEL:
                            {
                                for (int i = 0; i < channels.Count; i++)
                                    if (channels[i].Name == bmp.parameters[0].ToString())
                                        for (int c = 0; c < users.Count; c++)
                                            if (users[c].Login == bmp.sender)
                                            {
                                                channels[i].Add(users[c]);
                                                s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_JOIN_CHAT_CHANNEL, null, new object[] { true })));
                                                Console.WriteLine();
                                                s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                                                return;
                                            }
                                s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_JOIN_CHAT_CHANNEL, null, new object[] { false })));
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_GET_CHAT_CHANNELS:
                            {
                                List<string> chan = new List<string>();
                                for (int i = 0; i < channels.Count; i++)
                                    if (channels[i].Contain(bmp.parameters[0].ToString()))
                                        chan.Add(channels[i].Name);
                                s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_GET_CHAT_CHANNELS, null, new object[] { chan })));
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_MESSAGE:
                            {
                                for (int i = 0; i < channels.Count; i++)
                                    if (channels[i].Name == bmp.parameters[0].ToString())
                                        for (int c = 0; c < channels[i].users.Count; c++)
                                            channels[i].users[c].socket.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_MESSAGE, null, new object[] { channels[i].Name, bmp.sender, bmp.parameters[1] })));
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_START_GAME:
                            {
                                if (users.Count == 2)
                                {
                                    Party p = new Party();
                                    p.Add(users[0]);
                                    p.Add(users[1]);
                                    parties.Add(p);
                                    p.StartGame();
                                }
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_MOVE:
                            {
                                for(int i = 0; i < parties.Count; i ++)
                                    if (parties[i].Contain(bmp.sender) != -1)
                                    {
                                        parties[i].Command(new Message(MESSAGE_TYPE.SERVER_ANSWER_MOVE, null, new object[] { parties[i].Contain(bmp.sender), bmp.parameters[0], bmp.parameters[1] }));
                                        s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                                        return;
                                    }
                                break;
                            }
                    }
                    s.BeginReceive(buf, 0, 1024, SocketFlags.None, ReceiveCallback, s);
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                User u = new User("");
                for(int i = 0; i < users.Count; i ++)
                    if (users[i].socket == s)
                    {
                        u = users[i];
                        break;
                    }
                Console.WriteLine(ex.Message);
                Console.WriteLine("Disconnect: " + u.Login);
                ServerLog("Disconnect: " + u.Login);
                users.Remove(u);
                UpdateOnlineList();
            }
        }
        void UpdateChatChannels()
        {
            for (int i = 0; i < users.Count; i++)
            {
                List<string> ch = new List<string>();
                for (int c = 0; c < channels.Count; c++)
                    if (channels[c].Contain(users[i].Login))
                        ch.Add(channels[c].Name);
                Message mes = new Message(MESSAGE_TYPE.SERVER_ANSWER_GET_CHAT_CHANNELS, null, ch.ToArray<object>());
                users[i].socket.Send(Message.GetBytes(mes));
            }
        }
        void UpdateOnlineList()
        {
            List<string> us = new List<string>();
            for (int i = 0; i < users.Count; i++)
                us.Add(users[i].Login);
            for (int i = 0; i < users.Count; i++)
            {
                Message mes = new Message(MESSAGE_TYPE.SERVER_ANSWER_GET_ONLINE_LIST, null, us.ToArray<object>());
                users[i].socket.Send(Message.GetBytes(mes));
            }
        }
        void ServerLog(string message)
        {
            StreamWriter st = new StreamWriter("ServerLog.txt", true);
            st.WriteLine(DateTime.Now + ": " + message);
            st.Flush();
            st.Close();
        }
        User Get_User(string login)
        {
            for (int i = 0; i < users.Count; i++)
                if (users[i].Login == login)
                    return users[i];
            return new User("");
        }
    }
}
