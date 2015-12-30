using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using System.Text;
using Library;

public static class Client
{
    ////
    public delegate void ServerMessageDelegate(Message m);
    //
    static public event ServerMessageDelegate Logined;
    static public event ServerMessageDelegate GotOnlineList;
    static public event ServerMessageDelegate GotChannelList;
    static public event ServerMessageDelegate GotMessage;
    static public event ServerMessageDelegate GameStarted;
    static public event ServerMessageDelegate Moved;
    ////
    static byte[] buffer = new byte[1024];
    static Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
    static IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2014);

    public static string login;

    static public void ReinitializeSocket()
    {
        s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
    }

    static public void Start(string login)
    {
        try
        {
            Client.login = login;
            s.BeginConnect(ip, ConnectCallback, s);
        }
        catch (Exception)
        {
            
        }
        
    }

    static void ConnectCallback(IAsyncResult result)
    {
        Send(Message.GetBytes(new Message(MESSAGE_TYPE.REQUEST_LOGIN, Client.login, null)));
        Logined += new ServerMessageDelegate(Client_Logined);
        s.BeginReceive(buffer, 0, 1024, SocketFlags.None, RecieveCallback, s);
    }

    static void Client_Logined(Message m)
    {
        if ((bool)m.parameters[0])
        {
            LoginScreen.ConfirmedLogin((bool)m.parameters[0]);
            List<string> online = m.parameters[1] as List<string>;
            RoomFindingScreen.players.Clear();
            foreach (var v in online)
            {
                RoomFindingScreen.players.Add(v.ToString());
            }

            string[] channels = new string[] { m.parameters[2] as string };
            ChatChannel.Synchronize(channels, RoomFindingScreen.chatChannels);
        }
        else
        {
            LoginScreen.ConfirmedLogin((bool)m.parameters[0]);
        }
    }

    static void RecieveCallback(IAsyncResult result)
    {
        Socket s = result.AsyncState as Socket;
        try
        {
            int count = s.EndReceive(result);
            if (count > 0)
            {
                Message m = Message.GetData(buffer);
                switch (m.type)
                {
                    case MESSAGE_TYPE.SERVER_ANSWER_LOGIN:
                        {
                            Logined(m);
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_GET_ONLINE_LIST:
                        {
                            RoomFindingScreen.players.Clear();
                            foreach (var v in m.parameters)
                            {
                                RoomFindingScreen.players.Add(v.ToString());   
                            }
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_GET_CHAT_CHANNELS:
                        {
                            ChatChannel.Synchronize(m.parameters, RoomFindingScreen.chatChannels);
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_MESSAGE:
                        {
                            int index = ChatChannel.GetIndex(m.parameters[0], RoomFindingScreen.chatChannels);
                            RoomFindingScreen.chatChannels[index].AddMessage(m.parameters[1], m.parameters[2]);
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_CREATE_CHAT_CHANNEL:
                        {
                            if ((bool)m.parameters[0])
                            {
                                ChannelMenu.confirmedCreation = true;
                            }
                            else
                            {
                                ChannelMenu.answer = "\n(try another channel name)";
                                ChannelMenu.failedCreation = true;
                            }
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_JOIN_CHAT_CHANNEL:
                        {
                            if ((bool)m.parameters[0])
                            {
                                ChannelMenu.confirmedJoin = true;
                            }
                            else
                            {
                                ChannelMenu.answer = "\n(try another channel name)";
                                ChannelMenu.confirmedJoin = true;
                            }
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_LEAVE_CHAT_CHANNEL:
                        {
                            if ((bool)m.parameters[0])
                            {
                                ChannelMenu.confirmedLeave = true;
                            }
                            else
                            {
                                ChannelMenu.answer = "\n(try another channel name)";
                                ChannelMenu.confirmedLeave = true;
                            }
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_START_GAME:
                        {
                            if ((bool)m.parameters[0] == true)
                            {
                                GameStarted(m);
                            }
                            break;
                        }
                    case MESSAGE_TYPE.SERVER_ANSWER_MOVE:
                        {
                            Moved(m);
                            break;
                        }
                }
                s.BeginReceive(buffer, 0, 1024, SocketFlags.None, RecieveCallback, s);
            }
        }
        catch (Exception exception)
        {
            //Output.Write(exception);
        }
    }

    static public void Send(byte[] message)
    {
        try
        {
            s.Send(message);
        }
        catch (Exception exception)
        {
            //Output.Write(exception);
        }
    }
}
