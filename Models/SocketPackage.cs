using System;
using System.Text;

namespace Models
{
    // 設一個類別是讓ChattingRoom的Client & Server可以便利的設定欲傳遞的資料
    public class SocketPackage
    {
        #region [屬性]
        public EnumAction Action { get; set; }  // 標頭
        public string Data { get; set; }        // 資料
        #endregion

        // ===============================================
        // 方法
        // ===============================================
        
        #region [方法][把SocketPackage轉換成byte[]]
        public static byte[] Package(SocketPackage package)
        {
            byte[] data = null;
            if (package.Data != null && package.Data != String.Empty)
            {
                data = Encoding.Default.GetBytes(package.Data);
            }
            byte[] result = (data != null) ? new byte[4 + data.Length] : new byte[4];
            byte[] action = BitConverter.GetBytes((int)package.Action);
            Array.Copy(action, 0, result, 0, 4);
            if (data != null)
            {
                Array.Copy(data, 0, result, 4, data.Length);
            }

            return result;
        }
        #endregion

        #region [方法][把byte[]還原成SocketPackage]
        public static SocketPackage Unpackage(byte[] stream)
        {
            bool has_data = (stream.Length - 4 > 0) ? true : false;
            byte[] action = new byte[4];
            byte[] data = (has_data) ? new byte[stream.Length - 4] : null;

            Array.Copy(stream, 0, action, 0, 4);
            if (has_data)
                Array.Copy(stream, 4, data, 0, stream.Length - 4);

            return new SocketPackage()
            {
                Action = (EnumAction)BitConverter.ToInt32(action, 0),
                Data = (data != null) ? Encoding.Default.GetString(data) : String.Empty
            };
        }
        #endregion

        // ===============================================
        // 列舉型別
        // ===============================================

        #region [列舉型別][定義封包的標頭]
        public enum EnumAction
        {
            AuthenticationRequest,
            AuthenticationResponsive,
            ClientSendMessage,
            ClientReciveMessage
        }
        #endregion
    }
}
