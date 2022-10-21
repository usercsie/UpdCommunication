using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib
{
    public enum ConnectorErrorCode
    {
        NOERROR = 0,        
        CLIENT_SEND_FAILED,                 
        CLIENT_RECEIVE_FAILED,              
        SERVER_SEND_FAILED,                 
        BROADCASE_FAILED,
    }
}
