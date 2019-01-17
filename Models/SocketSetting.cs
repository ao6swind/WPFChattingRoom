using Modules;

namespace Models
{
    public class SocketSetting
    {
        // Socket的連線設定，因為只是要Demo給同事看，所以做成靜態屬性
        // 如果要做成產品，可以在專案屬性裡面
        // 搭配網域解析成IP來用
        public static string IP { get { return Network.GetCurrentIP(); } }
        public static int Port { get { return 8888; } }
        public static int Number { get { return 5; } }
    }
}
