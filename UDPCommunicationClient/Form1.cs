using CommunicationLib;
using CommunicationLib.Broadcast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPCommunicationClient
{
    public partial class Form1 : Form
    {
        TextLogger _Logger = null;
        MyUdpClient _Client = null;

        ConnectorErrorCode _ErrorCode;
        public bool IsSearching { get; private set; }
        public int TimeOut { get; set; }

        private IPAddress[] BroadCaseIPAddresses
        {
            get
            {
                return SearchingPrinterHelper.GetBroadcastAddresses();
            }
        }


        public Form1()
        {
            InitializeComponent();

            _Logger = new TextLogger(txtMessage);
            _Client = new MyUdpClient();
            _Client.ErrorOccurred += _Client_ErrorOccurred;

            IsSearching = false;
            TimeOut = 5000;
        }

        private void _Client_ErrorOccurred(object sender, ConnectorErrorEventArgs e)
        {
            _ErrorCode = e.Code;
        }

        private void btnBroadcase_Click(object sender, EventArgs e)
        {
            btnBroadcase.Enabled = false;

            //IPEndPoint BroadcastIp = new IPEndPoint(IPAddress.Broadcast, Convert.ToInt32(txtPortNumber.Text));
            //ConnectorErrorCode err = SLSwareConnector.Instance.SearchPrinters(BroadcastIp);
            ConnectorErrorCode err = Broadcase(Convert.ToInt32(txtPortNumber.Text));
            if (err == ConnectorErrorCode.NOERROR)
            {
                _Logger.Info("Broadcasting...");
            }
            else
            {
                _Logger.Warning(string.Format("Failed to broadcase:{0}", err));                
            }
            btnBroadcase.Enabled = true;
        }

        private ConnectorErrorCode Broadcase(int port)
        {
            _ErrorCode = ConnectorErrorCode.NOERROR;

            foreach (var ip in BroadCaseIPAddresses)
            {
                byte[] data = Encoding.ASCII.GetBytes("Broadcasting...");
                if (_Client.Send(new IPEndPoint(ip, port), data) == false)
                {
                    if (_ErrorCode != ConnectorErrorCode.NOERROR)
                    {
                        return _ErrorCode;
                    }
                    else
                        return ConnectorErrorCode.BROADCASE_FAILED;
                }

                if (IsSearching == false)
                {
                    //create a thread to wait printers' response.                
                    Thread thread = new Thread(Accept);
                    thread.Start();
                    IsSearching = true;
                }
            }

            return ConnectorErrorCode.NOERROR;
        }

        private void Accept()
        {
            _Client.ReadTimeOut = TimeOut;
            int number = 0;
            while (IsSearching)
            {
                byte[] result;
                IPEndPoint ip;
                if (_Client.Read(out result, out ip) == true)
                {
                    _Logger.Info(string.Format("received response from server: {0} bytes, ip:{1}", result.Length, ip));
                    number++;
                }
                else
                    break;
            }
            IsSearching = false;
            OnBroadcastFinished();
        }

        private void OnBroadcastFinished()
        {
            if (InvokeRequired == true)
            {
                Action a = new Action(OnBroadcastFinished);
                btnBroadcase.Invoke(a);
            }
            else
            {
                btnBroadcase.Enabled = true;
            }
        }
    }
}
