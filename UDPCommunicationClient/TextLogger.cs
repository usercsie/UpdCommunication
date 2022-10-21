using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDPCommunicationClient
{
    class TextLogger
    {
        private TextBox _TextBox;
        public TextLogger(TextBox txt)
        {
            _TextBox = txt;
        }

        public void Info(string message)
        {
            if (_TextBox.InvokeRequired == true)
            {
                Action<string> a = new Action<string>(Info);
                _TextBox.Invoke(a, message);
            }
            else
                _TextBox.AppendText(string.Format("[Info] {0}-{1}" + Environment.NewLine, DateTime.Now, message));
        }

        public void Warning(string message)
        {
            if (_TextBox.InvokeRequired == true)
            {
                Action<string> a = new Action<string>(Warning);
                _TextBox.Invoke(a, message);
            }
            else
                _TextBox.AppendText(string.Format("[Warning] {0}-{1}" + Environment.NewLine, DateTime.Now, message));
        }
    }
}
