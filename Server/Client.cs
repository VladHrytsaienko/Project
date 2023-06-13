using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Server.Net.IO;

namespace Server
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _PacketReader;
        public Client(TcpClient client) 
        {
            ClientSocket = client;
            UID=Guid.NewGuid();
            _PacketReader=new PacketReader(ClientSocket.GetStream());
            var opcode=_PacketReader.ReadByte();
            Username = _PacketReader.ReadMessage();
            Console.WriteLine($"[{DateTime.Now}]:Client has connecte with the username:{Username}");
            Task.Run(() => Process());
        }
        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _PacketReader.ReadByte();
                    switch (opcode) 
                    {
                        case 5:
                            var msg = _PacketReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]:Message received!{msg}");
                            Program.BroadcatMessage($"[{DateTime.Now}]:[{Username}]:{msg}");
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]:Disconected!");
                    Program.BroadcatDisconect(UID.ToString());
                    ClientSocket.Close();
                    break;

                }
            }
        }
    }
}
