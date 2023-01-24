using Server.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users =new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();
            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);
                BroadcastConnetion();
            }
        }
        static void BroadcastConnetion()
        {
            foreach(var user in _users)
            {
                foreach(var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());

                }
            }
        }
        public static void  BroadcatMessage(string Message)
        {
            foreach(var user in _users)
            {
                var msgPacket=new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(Message);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }
        public static void BroadcatDisconect(string uid)
        {   var disconnectedUser= _users.Where(x=>x.UID.ToString()==uid).FirstOrDefault();
            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
            BroadcatMessage($"[{disconnectedUser.Username}] Disconnected");
        }
    }
}