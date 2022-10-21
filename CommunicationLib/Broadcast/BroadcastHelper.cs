using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationLib.Broadcast
{
    public class SearchingPrinterHelper
    {
        public static IPAddress[] GetLocalAddresses()
        {
            return Dns.GetHostAddresses(Dns.GetHostName()).Where(v => v.AddressFamily == AddressFamily.InterNetwork).ToArray();
        }

        public static IPAddress[] GetBroadcastAddresses()
        {
            List<IPAddress> result = new List<IPAddress>();
            var hostAddresses = Dns.GetHostAddresses(Dns.GetHostName()).Where(v => v.AddressFamily == AddressFamily.InterNetwork);
            foreach (IPAddress IPA in hostAddresses)
            {
                UnicastIPAddressInformation info = GetIPAddressInfo(IPA);
                if (info != null)
                {
                    result.Add(GetBroadcastAddress(info));
                }
            }
            return result.ToArray();
        }

        public static IPAddress GetBroadcastAddress(UnicastIPAddressInformation unicastAddress)
        {
            return GetBroadcastAddress(unicastAddress.Address, unicastAddress.IPv4Mask);
        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
        }

        private static UnicastIPAddressInformation GetIPAddressInfo(IPAddress ip)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                UnicastIPAddressInformation info = adapter.GetIPProperties().UnicastAddresses.Where(v => v.Address.AddressFamily == AddressFamily.InterNetwork)
                                                                                             .Where(v => v.Address.ToString() == ip.ToString())
                                                                                             .FirstOrDefault();
                if (info != null)
                    return info;
            }

            return null;
        }
    }
}
