using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public class UdpDataReceivedEventArgs : EventArgs
    {
        public IPEndPoint Ip { get; private set; }

        public byte[] Buffer { get; private set; }

        public UdpDataReceivedEventArgs(IPEndPoint ip, byte[] data)
        {
            Ip = ip;
            Buffer = data;
        }
    }
}
