using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Modules
{
    public class Network
    {
        public static string GetCurrentIP()
        {
            string host = Dns.GetHostName();
            IPAddress[] ipAddresses = Dns.GetHostAddresses(host);
            return ipAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork).ToString();
        }
    }
}
