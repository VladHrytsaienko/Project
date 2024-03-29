﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

namespace Server.Net.IO
{
    class PacketReader :BinaryReader
    {
        private NetworkStream _ns;
        public PacketReader(NetworkStream ns):base(ns)
        {
            _ns = ns;
        }
        ASCIIEncoding ascii = new ASCIIEncoding();
        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer= new byte[length];
            _ns.Read(msgBuffer, 0, length);
            var msg = ascii.GetString(msgBuffer);
            return msg;
        }
    }
}
