using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SocketServer : INotifyPropertyChanged
    {
        // 事件
        public delegate void SocketServerInitSuccessHandler(SocketServer server);
        public delegate void SocketServerInitFailedHandler(SocketServer server, Exception ex);
        public delegate void SocketServerCloseSuccessHandler(SocketServer server);
        public delegate void SocketServerCloseFailedHandler(SocketServer server, Exception ex);
        public delegate void SocketServerAcceptSuccessHandler(SocketServer server, SocketClient client);
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

        // 欄位
        private Socket _server;
        private List<SocketClient> _clients = new List<SocketClient>();

        // 屬性
        public bool IsListening { get; private set; }   // 伺服器狀態
        public int OnlineMember { get; private set; }   // 在線人數

        // 建構式
        public SocketServer()
        {
            IsListening = false;
        }

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

        private void _accept_callback(IAsyncResult AR)
        {
            try
            {
                byte[] data;
                Socket client = _server.EndAccept(out data, AR);

                SocketClient accepted = new SocketClient()
                {
                    User = JsonConvert.DeserializeObject<User>(Encoding.Default.GetString(data))
                };

                accepted.OnSocketClientReceivedSuccess += Accepted_OnSocketClientReceivedSuccess;
                accepted.OnSocketClientReceivedFailed += Accepted_OnSocketClientReceivedFailed;

                this._clients.Add(accepted);
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

        private void Accepted_OnSocketClientReceivedSuccess(SocketClient sender, byte[] data)
        {
            if (OnSocketServerReciveSuccess != null)
            {
                OnSocketServerReciveSuccess(this, data);
            }
        }

        private void Accepted_OnSocketClientReceivedFailed(SocketClient sender, Exception ex)
        {
            if (OnSocketServerReciveFailed != null)
            {
                OnSocketServerReciveFailed(this, ex);
            }
        }
    }
}
