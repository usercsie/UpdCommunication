using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public class MyUdpClient : IUdpClientConnector
    {
        public event EventHandler<ConnectorErrorEventArgs> ErrorOccurred;

        private UdpClient _Udp;

        public int SendTimeOut
        {
            get; set;
        }
        public int ReadTimeOut
        {
            get; set;
        }

        public int PortNumber { get; private set; }

        public MyUdpClient()
        {
            SendTimeOut = 0;
            ReadTimeOut = 0;

            _Udp = new UdpClient();
            _Udp.EnableBroadcast = true;
        }
        ~MyUdpClient()
        {
            Close();
        }

        public bool Send(IPEndPoint ip, byte[] data)
        {
            ConsoleLogger.Info(string.Format("Send data: {0} bytes to {1}.",  data.Length, ip));
            int len = 0;
            try
            {
                PortNumber = ip.Port;
                _Udp.Client.SendTimeout = SendTimeOut;
                len = _Udp.Send(data, data.Length, ip);
            }
            catch (Exception e)
            {
                RaiseExceptionMessageNotified(ConnectorErrorCode.CLIENT_SEND_FAILED, "Failed to send data.", e);
            }

            return len == data.Length;
        }

        public bool Read(out byte[] data, out IPEndPoint ip)
        {
            data = new byte[0];

            ip = new IPEndPoint(IPAddress.Broadcast, PortNumber);
            _Udp.Client.ReceiveTimeout = ReadTimeOut;
            try
            {
                data = _Udp.Receive(ref ip);
                return true;
            }
            catch (SocketException se)
            {
                RaiseExceptionMessageNotified(ConnectorErrorCode.CLIENT_RECEIVE_FAILED, "Failed to read data, ErrorCode:" + se.ErrorCode, se);
                return false;
            }
            catch (Exception e)
            {
                RaiseExceptionMessageNotified(ConnectorErrorCode.CLIENT_RECEIVE_FAILED, "Failed to read data.", e);
                return false;
            }
        }

        public void Close()
        {
            if (_Udp != null)
            {
                _Udp.Close();
                _Udp = null;
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
    }
}
