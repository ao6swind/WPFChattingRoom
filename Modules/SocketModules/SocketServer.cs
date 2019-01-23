using Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Modules.SocketModules
{
    public class SocketServer
    {
        #region [欄位]
        private bool _is_turn_on = false;
        private Socket _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        #endregion

        #region [屬性]
        public int AllowConnection { get { return SocketSetting.AllowConnection; } }
        public int Port { get { return SocketSetting.Port; } }
        public string IpAddress { get { return (_is_turn_on) ? ((IPEndPoint)_server.LocalEndPoint).Address.ToString() : String.Empty; } }
        public string Status { get { return (_is_turn_on) ? "Running" : "Stoped"; } }
        public int OnlineMember { get { return Connections.Count; } }
        public List<SocketConnection> Connections { get; set; }
        #endregion

        #region [事件]
        public delegate void SocketServerAcceptedSuccessHandler(SocketServer sender, Socket accept);
        public delegate void SocketServerAcceptedFailedHandler(SocketServer sender, Exception ex);
        public delegate void SocketServerSendSuccessHandler(SocketServer sender, User user, SocketPackage package);
        public delegate void SocketServerSendFailedHandler(SocketServer sender, User user, SocketPackage package, Exception ex);

        public event SocketServerAcceptedSuccessHandler OnSocketServerAcceptedSuccess;
        public event SocketServerAcceptedFailedHandler OnSocketServerAcceptedFailed;
        public event SocketServerSendSuccessHandler OnSocketServerSendSuccess;
        public event SocketServerSendFailedHandler OnSocketServerSendFailed;
        #endregion

        #region [建構式]
        public SocketServer()
        {
            Connections = new List<SocketConnection>();
            _server.IOControl(IOControlCode.KeepAliveValues, _keep_alive(1, 1000, 1000), null);
            _server.Bind(new IPEndPoint(IPAddress.Parse(SocketSetting.IpAddress), SocketSetting.Port));
            _server.Listen(SocketSetting.AllowConnection);
        }
        #endregion

        // =======================================
        // 方法
        // =======================================

        #region [方法][啟動伺服器監聽]
        public void Start()
        {
            _is_turn_on = true;
            _server.BeginAccept(_accept_callback, this);
        }
        #endregion

        #region [方法][停止伺服器監聽]
        public void Stop()
        {
            _is_turn_on = false;
            foreach (SocketConnection connection in Connections)
            {
                connection.Close();
            }
            Connections.Clear();
        }
        #endregion

        #region [方法][傳送給指定使用者]
        public void SendTo(User user, SocketPackage package)
        {
            try
            {
                Connections.Find(x => x.User == user).Send(package);
                OnSocketServerSendSuccess?.Invoke(this, user, package);
            }
            catch (Exception ex)
            {
                OnSocketServerSendFailed?.Invoke(this, user, package, ex);
            }
        }
        #endregion

        #region [方法][廣播]
        public void Broadcast(SocketPackage package)
        {
            foreach(SocketConnection connection in Connections)
            {
                connection.Send(package);
            }
        }
        #endregion

        // =======================================
        // Callback
        // =======================================

        #region [Callback][接受連線成功時]
        private void _accept_callback(IAsyncResult AR)
        {
            try
            {
                if (_is_turn_on)
                {
                    Socket accept = _server.EndAccept(AR);
                    _server.BeginAccept(_accept_callback, this);

                    // 接收連線成功委派
                    OnSocketServerAcceptedSuccess?.Invoke(this, accept);
                }
            }
            catch (Exception ex)
            {
                // 接收連線失敗委派
                OnSocketServerAcceptedFailed?.Invoke(this, ex);
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
