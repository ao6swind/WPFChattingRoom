using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SocketClient
    {
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

        private Socket _client { get; set; }
        public User User { get; set; }

        public SocketClient()
        {
            
        }

        public void Connect(User user)
        {
            try
            {
                byte[] data = Encoding.Default.GetBytes(JsonConvert.SerializeObject(user));
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.UserToken = _client;
                e.SetBuffer(data, 0, data.Length);
                e.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(SocketSetting.IP), SocketSetting.Port);
                e.Completed += new EventHandler<SocketAsyncEventArgs>(_connect_completed);
                _client.ConnectAsync(e);
            }
            catch (Exception ex)
            {

            }
        }

        private void _connect_completed(object sender, SocketAsyncEventArgs e)
        {
            if (_client.Connected)
            {
                OnSocketClientConnectSuccess(this);
                _client.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
            }
            else
            {
                OnSocketClientConnectFailed(this, e.SocketError.ToString());
            }
        }

        private void _received_callback(IAsyncResult AR)
        {
            try
            {
                _client.EndReceive(AR);
                byte[] buffer = new byte[1024];

                int received = _client.Receive(buffer, buffer.Length, 0);

                if (received < buffer.Length)
                {
                    Array.Resize<byte>(ref buffer, received);
                }

                OnSocketClientReceivedSuccess(this, buffer);

                if (_client.Connected)
                    _client.BeginReceive(new byte[] { 0 }, 0, 0, 0, _received_callback, null);
            }
            catch (Exception ex)
            {
                OnSocketClientReceivedFailed(this, ex);
            }
        }
    }
}
