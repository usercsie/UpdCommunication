using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public interface IUdpServerConnector
    {
        event EventHandler<ConnectorErrorEventArgs> ErrorOccurred;
        event EventHandler<UdpDataReceivedEventArgs> DataReceived;

        int SendTimeOut { get; set; }
        bool IsRunning { get; }

        void Start(int portNumber);
        bool Send(IPEndPoint ip, byte[] data);
        void Stop();
    }
}
