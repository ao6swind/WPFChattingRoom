using Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Modules.SocketModules
{
    public class SocketClient
    {
        #region [欄位]
        private Socket _client;
        private User _user;
        #endregion

        #region [屬性]
        public string IpAddress { get { return (_client != null) ? ((IPEndPoint)_client.LocalEndPoint).Address.ToString() : String.Empty; } }
        public bool Connected { get { return (_client != null) ? _client.Connected : false; } }
        public User User { get { return _user; } }
        #endregion

        #region [事件]
        public delegate void SocketClientReceivedSuccessHandler(SocketClient sender, byte[] data);
        public delegate void SocketClientReceivedFailedHandler(SocketClient sender, Exception ex);
        public delegate void SocketClientConnectSuccessHandler(SocketClient sender);
        public delegate void SocketClientConnectFailedHandler(SocketClient sender, Exception ex);
        public delegate void SocketClientSendSuccessHandler(SocketClient sender, SocketPackage package);
        public delegate void SocketClientSendFailedHandler(SocketClient sender, Exception ex);
        public delegate void SocketClientCloseSuccessHandler(SocketClient sender);
        public delegate void SocketClientCloseFailedHandler(SocketClient sender, Exception ex);

        public event SocketClientReceivedSuccessHandler OnSocketClientReceivedSuccess;
        public event SocketClientReceivedFailedHandler OnSocketClientReceivedFailed;
        public event SocketClientConnectSuccessHandler OnSocketClientConnectSuccess;
        public event SocketClientConnectFailedHandler OnSocketClientConnectFailed;
        public event SocketClientSendSuccessHandler OnSocketClientSendSuccess;
        public event SocketClientSendFailedHandler OnSocketClientSendFailed;
        public event SocketClientCloseSuccessHandler OnSocketClientCloseSuccess;
        public event SocketClientCloseFailedHandler OnSocketClientCloseFailed;
        #endregion

        #region [建構式]
        public SocketClient(User user)
        {
            _user = user;
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _client.IOControl(IOControlCode.KeepAliveValues, _keep_alive(1, 1000, 1000), null);
        }
        #endregion

        // =======================================
        // 方法
        // =======================================

        #region [方法][連線到Socket server]
        public void Connect()
        {
            // 設定非同步連線
            byte[] data = Encoding.Default.GetBytes(JsonConvert.SerializeObject(_user));
            SocketAsyncEventArgs e = new SocketAsyncEventArgs();
            e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(SocketSetting.IpAddress), SocketSetting.Port);
            e.UserToken = _client;
            e.SetBuffer(data, 0, data.Length);
            e.Completed += new EventHandler<SocketAsyncEventArgs>(_connect_completed);
            _client.ConnectAsync(e);
        }
        #endregion

        #region [方法][結束連線]
        public void Disonnect()
        {
            _client.Close();
        }
        #endregion

        #region [方法][傳送資料]
        public void Send(SocketPackage package)
        {
            try
            {
                if (_client != null)
                {
                    _client.Send(SocketPackage.Stream(package));
                    OnSocketClientSendSuccess?.Invoke(this, package);
                }
            }
            catch (Exception ex)
            {
                OnSocketClientSendFailed?.Invoke(this, ex);
            }
        }
        #endregion

        // =======================================
        // Callback
        // =======================================

        #region [Callback][連線成功時]
        private void _connect_completed(object sender, SocketAsyncEventArgs e)
        {
            _client.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
        }
        #endregion

        #region [Callback][收到資料時]
        private void _received_callback(IAsyncResult AR)
        {
            try
            {
                if (_client != null)
                {
                    _client.EndReceive(AR);
                    byte[] buffer = new byte[1024];

                    int received = _client.Receive(buffer, buffer.Length, 0);

                    if (received < buffer.Length)
                    {
                        Array.Resize<byte>(ref buffer, received);
                    }

                    OnSocketClientReceivedSuccess?.Invoke(this, buffer);

                    if (_client.Connected)
                        _client.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
                }
            }
            catch (Exception ex)
            {
                OnSocketClientReceivedFailed?.Invoke(this, ex);
            }
        }
        #endregion

        // =======================================
        // 私有函式
        // =======================================

        #region [處理]設定偵測socket連線之參數
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
