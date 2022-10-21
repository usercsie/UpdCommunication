using CommunicationLib;
using CommunicationLib.Broadcast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPCommunication
{
    public partial class Form1 : Form
    {
        TextLogger _Logger = null;
        MyUdpServer _UdpServer = null;

        public Form1()
        {
            InitializeComponent();

            _Logger = new TextLogger(txtMessage);

            InitAvailableLocalIpAddressCombobox();
        }

        private void InitAvailableLocalIpAddressCombobox()
        {
            cbAvailableIpAddress.Items.Clear();
            cbAvailableIpAddress.Items.Add("127.0.0.1");
            foreach (var ip in SearchingPrinterHelper.GetLocalAddresses())
            {
                cbAvailableIpAddress.Items.Add(ip.ToString());
            }
            cbAvailableIpAddress.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            if (StartUdpServer(Convert.ToInt32(txtPortNumber.Text)) == true)
            {              
                _Logger.Info(string.Format("Udp server started, port number: {0}", txtPortNumber.Text));
                btnStop.Enabled = true;
                btnStart.Enabled = false;
            }
            else
                _Logger.Warning("Server start failed.");
        }

        private bool StartUdpServer(int port)
        {
            _UdpServer = new MyUdpServer();
            _UdpServer.DataReceived += _UdpServer_DataReceived;
            _UdpServer.ErrorOccurred += _Server_ErrorOccurred;

            try
            {
                _UdpServer.Start(port);
                return true;
            }
            catch (Exception)
            {                
                return false;
            }            
        }

        private void _Server_ErrorOccurred(object sender, ConnectorErrorEventArgs e)
        {
            _Logger.Warning(string.Format("Code:{0},{1}:{2}", e.Code.ToString(), e.Description, e.Details));
        }

        private void _UdpServer_DataReceived(object sender, UdpDataReceivedEventArgs e)
        {
            _Logger.Info(string.Format("Data received:{0} bytes from {1}", e.Buffer.Length, e.Ip));

            _UdpServer.SendTimeOut = 5000;
            _UdpServer.Send(e.Ip, Encoding.ASCII.GetBytes("Reply data to client"));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopSafe();
        }

        private void StopSafe()
        {
            if (this.InvokeRequired == true)
            {
                Action a = new Action(StopSafe);
                this.Invoke(a);
            }
            else
            {
                _UdpServer.Stop();
                _Logger.Info(string.Format("Server stopped."));

                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }
    }
}
