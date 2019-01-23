using Models;
using System;
using System.Net;
using System.Net.Sockets;

namespace Modules.SocketModules
{
    public class SocketConnection
    {
        #region [欄位]
        private Socket _connection;
        #endregion

        #region [方法]
        public string IpAddress { get { return ((IPEndPoint)_connection.RemoteEndPoint).Address.ToString(); } }
        public User User { get; set; }
        #endregion

        #region [事件]
        public delegate void SocketConnectionReceivedSuccessHandler(SocketConnection cconnection, byte[] data);
        public delegate void SocketConnectionReceivedFailedHandler(SocketConnection cconnection, Exception ex);
        public delegate void SocketConnectionSendSuccessHandler(SocketConnection cconnection);
        public delegate void SocketConnectionSendFailedHandler(SocketConnection cconnection, Exception ex);
        public delegate void SocketConnectionCloseSuccessHandler(SocketConnection cconnection);
        public delegate void SocketConnectionCloseFailedHandler(SocketConnection cconnection, Exception ex);

        public event SocketConnectionReceivedSuccessHandler OnSocketConnectionReceivedSuccess;
        public event SocketConnectionReceivedFailedHandler OnSocketConnectionReceivedFailed;
        public event SocketConnectionSendSuccessHandler OnSocketConnectionSendSuccess;
        public event SocketConnectionSendFailedHandler OnSocketConnectionSendFailed;
        public event SocketConnectionCloseSuccessHandler OnSocketConnectionCloseSuccess;
        public event SocketConnectionCloseFailedHandler OnSocketConnectionCloseFailed;
        #endregion

        #region [建構式]
        public SocketConnection(Socket accepted)
        {
            _connection = accepted;
            _connection.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
        }
        #endregion

        // ===============================================
        // 方法
        // ===============================================

        #region [方法][傳遞資料給使用者]
        public void Send(SocketPackage package)
        {
            if (_connection.Connected)
            {
                byte[] data = SocketPackage.Stream(package);
                _connection.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(_send_callback), _connection);
            }
        }
        #endregion

        #region [方法][關閉伺服器端上監聽資料傳入的socket]
        public void Close()
        {
            _connection.Shutdown(SocketShutdown.Both);
            _connection.Disconnect(false);
            _connection.Close();
            _connection.Dispose();
        }
        #endregion

        // =======================================
        // Callback
        // =======================================

        #region [Callback][資料接收成功時]
        private void _received_callback(IAsyncResult AR)
        {
            try
            {
                if (_connection != null)
                {
                    _connection.EndReceive(AR);
                    byte[] buffer = new byte[1024 * 8];

                    int received = _connection.Receive(buffer, buffer.Length, 0);
                    if (received < buffer.Length)
                    {
                        Array.Resize<byte>(ref buffer, received);
                    }

                    OnSocketConnectionReceivedSuccess?.Invoke(this, buffer);

                    if (_connection.Connected)
                        _connection.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
                }
            }
            catch (Exception ex)
            {
                OnSocketConnectionReceivedFailed?.Invoke(this, ex);
            }
        }
        #endregion

        #region [Callback][資料傳送成功時]
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
    }
}
