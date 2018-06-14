using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarlockArena2DServer
{
    class ChatChannel
    {
        public string name;
        public List<string> members = new List<string>();
        public string owner;

        public ChatChannel(string name, string owner)
        {
            this.name = name;
            this.owner = owner;
            if (owner != "Server")
            {
                AddMember(owner);
            }
        }

        public bool AddMember(string name)
        {
            if (!members.Contains(name))
            {
                members.Add(name);
                return true;
            }else
            {
                return false;
            }
        }
    }
}
