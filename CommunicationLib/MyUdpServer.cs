using CommunicationLib.Core.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public class MyUdpServer : IUdpServerConnector
    {
        public event EventHandler<ConnectorErrorEventArgs> ErrorOccurred;
        public event EventHandler<UdpDataReceivedEventArgs> DataReceived;

        public int SendTimeOut { get; set; }
        public bool IsRunning
        {
            get { return _Udp != null && _Udp.IsRunning; }
        }

        private UdpServerAsync _Udp;

        public MyUdpServer()
        {
            _Udp = new UdpServerAsync();
            _Udp.DataReceived += _Udp_DataReceived;

            SendTimeOut = 0;
        }

        private void _Udp_DataReceived(object sender, UdpDataReceivedEventArgs e)
        {
            if (DataReceived != null)
            {
                DataReceived(this, e);
            }
        }

        public void Start(int portNumber = 50001)
        {
            _Udp.Start(portNumber);
        }
        public bool Send(IPEndPoint ip, byte[] data)
        {
            try
            {
                _Udp.Client.SendTimeout = SendTimeOut;
                _Udp.Send(ip, data);
                return true;
            }
            catch (Exception e)
            {
                RaiseExceptionMessageNotified(ConnectorErrorCode.SERVER_SEND_FAILED, "Failed to send data.", e);
                return false;
            }
        }
        private void RaiseExceptionMessageNotified(ConnectorErrorCode code, string message, Exception e)
        {
            EventHandler<ConnectorErrorEventArgs> handler = ErrorOccurred;
            if (handler != null)
            {
                handler(this, new ConnectorErrorEventArgs(code, message, e.Message));
            }
        }
        public void Stop()
        {
            _Udp.Stop();
        }
    }
}
