using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WarlockArena2DLibrary;

namespace WarlockArena2DServer
{
    class Server
    {
        Socket serverSocket;
        IPEndPoint serverAddress;
        List<Player> players = new List<Player>();
        List<ChatChannel> chatChannels = new List<ChatChannel>();

        byte[] buffer = new byte[1024];

        public delegate void MessageDelegate(string message);

        public event MessageDelegate GetMessage;
        public event MessageDelegate ChatChannelCountChanged;
        public event Action OnlineChanged;
        public event Action MessageSent;

        public Server()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                //serverAddress = new IPEndPoint(IPAddress.Parse("25.132.112.36"), 2014);
                serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.7"), 2014);
                serverSocket.Bind(serverAddress);
                serverSocket.Listen(10);
                serverSocket.BeginAccept(AcceptCallback, serverSocket);
                Console.Title = "Warlock Arena 2D Server";
                Console.WriteLine("> Server start: [" + DateTime.Now + "]");
                OnlineChanged += Server_OnlineChanged;
                ChatChannelCountChanged += Server_ChatChannelCountChanged;
                MessageSent += Server_MessageSent;
                chatChannels.Add(new ChatChannel("All", "Server"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        void Server_MessageSent()
        {
            
        }

        void Server_ChatChannelCountChanged(string message)
        {
            List<string> arr = new List<string>();
            foreach (var v in chatChannels)
            {
                if (v.members.Contains(message))
                {
                    arr.Add(v.name);
                }
            }
            GetPlayer(message).socket.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_GET_CHAT_CHANNELS, null, arr.ToArray())));
        }

        void Server_OnlineChanged()
        {
            List<string> arr = GetOnlineList();
            foreach (var v in players)
            {
                if (v.logged)
                {
                    v.socket.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_GET_ONLINE_LIST, null, arr.ToArray())));
                }
            }
        }
        //
        List<string> GetChatChannels(string player)
        {
            List<string> arr = new List<string>();
            foreach (var v in chatChannels)
            {
                if (v.members.Contains(player))
                {
                    arr.Add(v.name);
                }
            }
            return arr;
        }

        bool ChatChannelExists(string name)
        {
            foreach (var v in chatChannels)
            {
                if (v.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        int GetChatChannelIndex(string name)
        {
            for (int i = 0; i < chatChannels.Count; i++)
            {
                if (chatChannels[i].name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        int GetPlayerIndex(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        Player GetPlayer(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == name)
                {
                    return players[i];
                }
            }
            return null;
        }

        List<string> GetOnlineList()
        {
            List<string> arr = new List<string>();
            foreach (var v in players)
            {
                if (v.logged)
                {
                    arr.Add(v.name);
                }
            }
            return arr;
        }

        void AcceptCallback(IAsyncResult result)
        {
            try
            {
                Socket client = result.AsyncState as Socket;
                Socket s = client.EndAccept(result);
                if (GetMessage != null)
                {
                    players.Add(new Player(s));
                }
                s.BeginReceive(buffer, 0, 1024, SocketFlags.None, RecieveCallback, s);
                client.BeginAccept(AcceptCallback, client);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        void RecieveCallback(IAsyncResult result)
        {
            Socket s = result.AsyncState as Socket;
            try
            {
                int count = s.EndReceive(result);
                if (count > 0)
                {
                    Message m = Message.GetData(buffer);
                    string answer = "";
                    switch (m.type)
                    {
                        case MESSAGE_TYPE.REQUEST_LOGIN:
                            {
                                foreach (var v in players)
                                {
                                    if (v.name == m.sender)
                                    {
                                        answer = "> [" + s.RemoteEndPoint + "] failed logging in as '" + m.sender + "'. Reason: user already logged in.";
                                        s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_LOGIN, null, new object[] { false })));
                                        Console.WriteLine(answer);
                                        Error(s);
                                        return;
                                    }
                                }
                                answer = "> [" + s.RemoteEndPoint + "] logged in as '" + m.sender + "'.";
                                chatChannels[GetChatChannelIndex("All")].members.Add(m.sender);
                                foreach (var v in players)
                                {
                                    if (v.socket == s)
                                    {
                                        v.name = m.sender;
                                        v.logged = true;
                                        s.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_LOGIN, null, new object[] { true, GetOnlineList(), GetChatChannels(m.sender) })));
                                        break;
                                    }
                                }
                                ChatChannelCountChanged(m.sender);
                                OnlineChanged();
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_GET_ONLINE_LIST:
                            {
                                OnlineChanged();
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_CREATE_CHAT_CHANNEL:
                            {
                                chatChannels.Add(new ChatChannel(m.parameters[0].ToString(), m.sender));
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_JOIN_CHAT_CHANNEL:
                            {
                                if (ChatChannelExists(m.parameters[0].ToString()))
                                {
                                    chatChannels[GetChatChannelIndex(m.parameters[0].ToString())].AddMember(m.sender);
                                }
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_MESSAGE:
                            {
                                int index = GetChatChannelIndex(m.parameters[0].ToString());
                                foreach (var v in chatChannels[index].members)
                                {
                                    Player p = GetPlayer(v);
                                    p.socket.Send(Message.GetBytes(new Message(MESSAGE_TYPE.SERVER_ANSWER_MESSAGE, null, new object[] { m.parameters[0], m.sender, m.parameters[1] })));
                                }
                                break;
                            }
                        case MESSAGE_TYPE.REQUEST_GET_CHAT_CHANNELS:
                            {
                                ChatChannelCountChanged(m.sender);
                                break;
                            }
                    }
                    if (answer != "")
                    {
                        Console.WriteLine(answer);
                    }
                    s.BeginReceive(buffer, 0, 1024, SocketFlags.None, RecieveCallback, s);
                }
                else
                {
                    Error(s);
                }
            }
            catch (Exception exception)
            {
                Error(s);
            }
        }

        void Error(Socket s)
        {
            Console.WriteLine("> [" + s.RemoteEndPoint + "] disconnected from server.");
            Player disconnected = new Player();
            foreach (var v in players)
            {
                if (v.socket == s)
                {
                    disconnected = v;
                }
            }
            int i = 0;
            int j = 0;
            for (i = 0; i < chatChannels.Count; i++)
            {
                for (j = 0; j < chatChannels[i].members.Count; j++)
                {
                    if (chatChannels[i].members[j] == disconnected.name)
                    {
                        goto End;
                    }
                }
            }
            goto EndWithoutChannel;
        End:
            chatChannels[i].members.RemoveAt(j);
            players.Remove(disconnected);
            OnlineChanged();
        EndWithoutChannel:
            players.Remove(disconnected);
            OnlineChanged();
        }
    }
}
