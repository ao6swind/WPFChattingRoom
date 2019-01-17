using System.Linq;
using System.Net;
using System.Net.Sockets;

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
