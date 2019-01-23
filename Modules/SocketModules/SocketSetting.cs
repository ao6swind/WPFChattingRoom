using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.SocketModules
{
    public class SocketSetting
    {
        public static int Port { get { return 8888; } }
        public static string IpAddress { get { return Network.GetCurrentIP(); } }
        public static int AllowConnection { get { return 5; } }
    }
}
