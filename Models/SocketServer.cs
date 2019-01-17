using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Models
{
    public class SocketServer
    {
        #region [事件]
        public delegate void SocketServerInitSuccessHandler(SocketServer server);
        public delegate void SocketServerInitFailedHandler(SocketServer server, Exception ex);
        public delegate void SocketServerCloseSuccessHandler(SocketServer server);
        public delegate void SocketServerCloseFailedHandler(SocketServer server, Exception ex);
        public delegate void SocketServerAcceptSuccessHandler(SocketServer server, SocketClient accept);
        public delegate void SocketServerAcceptFailedHandler(SocketServer server, Exception ex);
        public delegate void SocketServerReciveSuccessHandler(SocketServer server, byte[] data);
        public delegate void SocketServerReciveFailedHandler(SocketServer server, Exception ex);

        public event SocketServerInitSuccessHandler OnSocketServerInitSuccess;
        public event SocketServerInitFailedHandler OnSocketServerInitFailed;
        public event SocketServerCloseSuccessHandler OnSocketServerCloseSuccess;
        public event SocketServerCloseFailedHandler OnSocketServerCloseFailed;
        public event SocketServerAcceptSuccessHandler OnSocketServerAcceptSuccess;
        public event SocketServerAcceptFailedHandler OnSocketServerAcceptFailed;
        public event SocketServerReciveSuccessHandler OnSocketServerReciveSuccess;
        public event SocketServerReciveFailedHandler OnSocketServerReciveFailed;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region [欄位]
        private Socket _server;
        private List<SocketClient> _clients = new List<SocketClient>();
        #endregion

        #region [屬性]
        public bool IsListening { get; private set; }   // 伺服器狀態
        public int OnlineMember { get { return _clients.Count; } }   // 在線人數
        #endregion

        #region [建構式]
        public SocketServer()
        {
            IsListening = false;
        }
        #endregion

        // ===============================================
        // 方法
        // ===============================================

        #region [方法][啟動伺服器]
        public void Start()
        {
            if (!IsListening)
            {
                try
                {
                    _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _server.Bind(new IPEndPoint(IPAddress.Parse(SocketSetting.IP), SocketSetting.Port));
                    _server.Listen(SocketSetting.Number);
                    _server.BeginAccept(_accept_callback, null);
                    IsListening = true;
                    if (OnSocketServerInitSuccess != null)
                    {
                        OnSocketServerInitSuccess(this);
                    }
                }
                catch (Exception ex)
                {
                    if (OnSocketServerInitFailed != null)
                    {
                        OnSocketServerInitFailed(this, ex);
                    }
                }
            }
        }
        #endregion

        #region [方法][停止伺服器]
        public void Stop()
        {
            if (IsListening)
            {
                IsListening = false;
                try
                {
                    _server.Close();

                    if (OnSocketServerCloseSuccess != null)
                    {
                        OnSocketServerCloseSuccess(this);
                    }
                }
                catch (Exception ex)
                {
                    if (OnSocketServerCloseFailed != null)
                    {
                        OnSocketServerCloseFailed(this, ex);
                    }
                }
            }
        }
        #endregion

        #region [方法][傳遞資料至指定使用者]
        public void Send(User user, string data)
        {
            
        }
        #endregion

        // ===============================================
        // callback處理函式
        // ===============================================

        #region [callback][接收連線]
        private void _accept_callback(IAsyncResult AR)
        {
            try
            {
                SocketClient accepted = new SocketClient()
                {
                    Client = _server.EndAccept(AR)
                };
                accepted.OnSocketClientReceivedSuccess += Accepted_OnSocketClientReceivedSuccess;
                accepted.OnSocketClientReceivedFailed += Accepted_OnSocketClientReceivedFailed;
                _clients.Add(accepted);
                _server.BeginAccept(_accept_callback, null);
                if (OnSocketServerAcceptSuccess != null)
                {
                    OnSocketServerAcceptSuccess(this, accepted);
                }
            }
            catch (Exception ex)
            {
                if (OnSocketServerAcceptFailed != null)
                {
                    OnSocketServerAcceptFailed(this, ex);
                }
            }
        }
        #endregion

        // ===============================================
        // 自定義事件處理函式
        // ===============================================

        #region [事件][接收到Client傳來的資料成功時]
        private void Accepted_OnSocketClientReceivedSuccess(SocketClient sender, byte[] data)
        {
            // 不管三七二十一直接丟出去給server的主視窗
            if (OnSocketServerReciveSuccess != null)
            {
                OnSocketServerReciveSuccess(this, data);
            }
        }
        #endregion

        #region [事件][接收到Client傳來的資料失敗時]
        private void Accepted_OnSocketClientReceivedFailed(SocketClient sender, Exception ex)
        {
            // 不管三七二十一直接丟出去給server的主視窗
            if (OnSocketServerReciveFailed != null)
            {
                OnSocketServerReciveFailed(this, ex);
            }
        }
        #endregion
    }
}
