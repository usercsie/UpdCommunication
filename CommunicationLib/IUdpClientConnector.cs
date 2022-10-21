using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public interface IUdpClientConnector
    {
        event EventHandler<ConnectorErrorEventArgs> ErrorOccurred;

        int SendTimeOut { get; set; }
        int ReadTimeOut { get; set; }
        int PortNumber { get; }
        
        bool Send(IPEndPoint ip, byte[] buffer);
        bool Read(out byte[] data, out IPEndPoint ip);
        void Close();
    }
}
