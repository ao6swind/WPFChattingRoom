using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Models
{
    public class SocketClient
    {
        #region [事件]
        public delegate void SocketClientConnectSuccessHandler(SocketClient sender);
        public delegate void SocketClientConnectFailedHandler(SocketClient sender, string msg);
        public delegate void SocketClientReceivedSuccessHandler(SocketClient sender, byte[] data);
        public delegate void SocketClientReceivedFailedHandler(SocketClient sender, Exception ex);
        public delegate void SocketClientCloseSuccessHandler(SocketClient sender);
        public delegate void SocketClientCloseFailedHandler(SocketClient sender, string msg);

        public event SocketClientConnectSuccessHandler OnSocketClientConnectSuccess;
        public event SocketClientConnectFailedHandler OnSocketClientConnectFailed;
        public event SocketClientReceivedSuccessHandler OnSocketClientReceivedSuccess;
        public event SocketClientReceivedFailedHandler OnSocketClientReceivedFailed;
        public event SocketClientCloseSuccessHandler OnSocketClientCloseSuccess;
        public event SocketClientCloseFailedHandler OnSocketClientCloseFailed;
        #endregion

        #region [屬性]
        public Socket Client { get; set; }
        public User User { get; set; }
        public string IpAddress
        {
            get
            {
                return (Client != null) ? ((IPEndPoint)Client.RemoteEndPoint).Address.ToString() : String.Empty;
            }
        }
        #endregion

        #region [建構式]
        public SocketClient()
        {
            
        }
        #endregion

        // ===============================================
        // 方法
        // ===============================================

        #region [方法][非同步連線至伺服器]
        public void Connect(User user)
        {
            try
            {
                byte[] data = Encoding.Default.GetBytes(JsonConvert.SerializeObject(user));
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Client.IOControl(IOControlCode.KeepAliveValues, _keep_alive(1, 1000, 1000), null);
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.UserToken = Client;
                e.SetBuffer(data, 0, data.Length);
                e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(SocketSetting.IP), SocketSetting.Port);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(_connect_completed);
                Client.ConnectAsync(e);
            }
            catch (Exception ex)
            {
                OnSocketClientConnectFailed(this, ex.Message.ToString());
            }
        }
        #endregion

        #region [方法][登出]
        public void Disonnect()
        {
            Client.Close();
        }
        #endregion

        #region [方法][非同步傳送至伺服器]
        public void Send(SocketPackage package)
        {
            if (Client.Connected)
            {
                byte[] data = SocketPackage.Package(package);
                Client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(_send_callback), Client);
            }
        }
        #endregion

        // ===============================================
        // callback處理函式
        // ===============================================

        #region [callback][連線完成]
        private void _connect_completed(object sender, SocketAsyncEventArgs e)
        {
            if (Client.Connected)
            {
                OnSocketClientConnectSuccess(this);
                Client.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
            }
            else
            {
                if (OnSocketClientConnectFailed != null)
                {
                    OnSocketClientConnectFailed(this, e.SocketError.ToString());
                }
            }
        }
        #endregion

        #region [callback][資料傳輸]
        private void _send_callback(IAsyncResult AR)
        {
            try
            {
                Socket connect = (Socket)AR.AsyncState;
                if (connect.Connected)
                    connect.EndSend(AR);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region [callback][資料接收]
        private void _received_callback(IAsyncResult AR)
        {
            try
            {
                Client.EndReceive(AR);
                byte[] buffer = new byte[1024];

                int received = Client.Receive(buffer, buffer.Length, 0);

                if (received < buffer.Length)
                {
                    Array.Resize<byte>(ref buffer, received);
                }

                OnSocketClientReceivedSuccess(this, buffer);

                if (Client.Connected)
                    Client.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
            }
            catch (Exception ex)
            {
                if (OnSocketClientReceivedFailed != null)
                {
                    OnSocketClientReceivedFailed(this, ex);
                }
            }
        }
        #endregion

        // ===============================================
        // 私有副函式
        // ===============================================

        #region [副函式]設定偵測socket連線之參數
        private byte[] _keep_alive(int enable, int keepAliveTime, int keepAliveInterval)
        {
            byte[] buffer = new byte[12];
            BitConverter.GetBytes(enable).CopyTo(buffer, 0);
            BitConverter.GetBytes(keepAliveTime).CopyTo(buffer, 4);
            BitConverter.GetBytes(keepAliveInterval).CopyTo(buffer, 8);
            return buffer;
        }
        #endregion
    }
}
