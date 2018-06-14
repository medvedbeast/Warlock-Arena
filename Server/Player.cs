using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace WarlockArena2DServer
{
    class Player
    {
        public Socket socket;
        public bool logged = false;
        public string name;

        public Player(Socket socket)
        {
            this.socket = socket;
            this.name = "";
        }

        public Player(Socket socket, string name)
        {
            this.socket = socket;
            this.name = name;
        }

        public Player()
        {

        }
    }
}
