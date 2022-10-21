using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib.Core.Sockets
{
    public class UdpServerAsync : IDisposable
    {
        private bool _Disposed = false;
        private int _PortNumber = 50001;
        private UdpClient _Udp;
        public Socket Client { get { return _Udp.Client; } }
        IAsyncResult _Result = null;

        public event EventHandler<UdpDataReceivedEventArgs> DataReceived;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        Stop();
                    }
                    catch (SocketException)
                    {
                    }
                }
                _Disposed = true;
            }
        }

        public bool IsRunning { get; private set; }
        public void Start(int portNumber = 50001)
        {
            if (IsRunning == false)
            {
                _PortNumber = portNumber;
                _Udp = new UdpClient(_PortNumber);
                StartListening();
                IsRunning = true;
                ConsoleLogger.Info("Started listening");
            }
        }
        private void StartListening()
        {
            _Result = _Udp.BeginReceive(Receive, new object());
        }
        private void Receive(IAsyncResult ar)
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, _PortNumber);
            byte[] bytes = new byte[0];

            try
            {
                bytes = _Udp.EndReceive(ar, ref ip);
            }
            catch (Exception e)
            {
                ConsoleLogger.Warning("Receive failure:" + e.Message);
                return;
            }
            string message = Encoding.ASCII.GetString(bytes);
            ConsoleLogger.Info(string.Format("From {0} received: {1} ", ip.Address.ToString(), message));
            StartListening();

            OnDataReceived(ip, bytes);
        }
        private void OnDataReceived(IPEndPoint ip, byte[] data)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new UdpDataReceivedEventArgs(ip, data));
            }
        }

        public void Stop()
        {
            if (IsRunning == true)
            {
                try
                {
                    IsRunning = false;
                    _Udp.Close();
                    Console.WriteLine("Stopped listening");
                }
                catch { /* don't care */ }
            }
        }
        public void Send(IPEndPoint ip, byte[] buffer)
        {
            UdpClient client = new UdpClient();

            try
            {
                client.Send(buffer, buffer.Length, ip);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                client.Close();
            }
            ConsoleLogger.Info(string.Format("Sent: {0} ", Encoding.ASCII.GetString(buffer)));
        }
    }
}
