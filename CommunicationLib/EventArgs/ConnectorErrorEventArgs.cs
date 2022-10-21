using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public class ConnectorErrorEventArgs : EventArgs
    {
        public ConnectorErrorCode Code { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }

        public ConnectorErrorEventArgs(ConnectorErrorCode code, string description, string details)
        {
            Code = code;
            Description = description;
            Details = details;
        }
    }
}
