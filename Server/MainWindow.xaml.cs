using Models;
using Models.DbContexts;
using Modules.SocketModules;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Server
{
    public partial class MainWindow : Window
    {
        #region [欄位]
        private SocketServer _server;
        #endregion

        #region [建構式]
        public MainWindow()
        {
            InitializeComponent();

            // 載入使用者清單
            ChattingRoomEntities db = new ChattingRoomEntities();
            pnlUserList.ItemsSource = db.Users.ToList();

            // 建立Socket Server
            _server = new SocketServer();
            _server.OnSocketServerAcceptedSuccess += _server_OnSocketServerAcceptedSuccess;
            _server.OnSocketServerAcceptedFailed += _server_OnSocketServerAcceptedFailed;

            // 啟動伺服器
            TurnOnServer();
        }       
        #endregion

        // ===============================================
        // UI事件處理函式
        // ===============================================

        #region [按鈕][左鍵][啟動伺服器]
        private void BtnTurnOn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TurnOnServer();
        }
        #endregion

        #region [按鈕][左鍵][關閉伺服器]
        private void BtnTurnOff_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _server.Broadcast(new SocketPackage()
            {
                Action = SocketPackage.EnumAction.ServerShutdown
            });
            _server.Stop();
            btnTurnOn.IsEnabled = true;
            btnTurnOff.IsEnabled = false;
            ShowInformation();
            ShowMessage("turn off server");
        }
        #endregion

        #region [按鈕][左鍵][廣播訊息]
        private void BtnSend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtMessage.Text != String.Empty)
            {
                ShowMessage(String.Format("broadcast: {0}", txtMessage.Text));
                _server.Broadcast(new SocketPackage()
                {
                    Action = SocketPackage.EnumAction.ServerBroadcast,
                    Data = txtMessage.Text
                });
                txtMessage.Text = String.Empty;
            }
        }
        #endregion

        // ===============================================
        // 自定義副函式
        // ===============================================

        #region [副函式][啟動伺服器]
        private void TurnOnServer()
        {
            _server.Start();
            btnTurnOn.IsEnabled = false;
            btnTurnOff.IsEnabled = true;
            ShowInformation();
            ShowMessage("turn on server");
        }
        #endregion

        #region [副函式][更新伺服器狀態]
        private void ShowInformation()
        {
            txtStatus.Content           = _server.Status;
            txtIpAddress.Content        = _server.IpAddress;
            txtPort.Content             = _server.Port;
            txtAllowConnection.Content  = _server.AllowConnection;
            txtOnlineMember.Content     = _server.OnlineMember;
        }
        #endregion

        #region [副函式][打印訊息]
        private void ShowMessage(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlMessage.Children.Add(new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = String.Format("{0} # {1}", DateTime.Now.ToString(), msg)
                });
            }));
        }
        #endregion

        // ===============================================
        // 自定義事件處理函式
        // ===============================================

        #region [SocketServer][接受連線失敗時]
        private void _server_OnSocketServerAcceptedFailed(SocketServer sender, Exception ex)
        {
            ShowMessage(String.Format("has error when accept connection: {0}", ex.Message));
        }
        #endregion

        #region [SocketServer][接受連線成功時]
        private void _server_OnSocketServerAcceptedSuccess(SocketServer sender, Socket accept)
        {
            SocketConnection connection = new SocketConnection(accept);
            connection.OnSocketConnectionReceivedSuccess += Connection_OnSocketConnectionReceivedSuccess;
            _server.Connections.Add(connection);

            ShowMessage(String.Format("{0} try to connect... ...", ((IPEndPoint)accept.RemoteEndPoint).Address.ToString()));
            accept.Send(SocketPackage.Stream(new SocketPackage()
            {
                Action = SocketPackage.EnumAction.AuthenticationRequest
            }));
        }
        #endregion

        #region [SocketConnection][接收資料成功時]
        private void Connection_OnSocketConnectionReceivedSuccess(SocketConnection connection, byte[] data)
        {
            if (data.Length > 0)
            {
                SocketPackage package = SocketPackage.Unpackage(data);

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    switch (package.Action)
                    {
                        /// =============================
                        /// 身分驗證成功
                        /// =============================
                        case SocketPackage.EnumAction.AuthenticationResponse:
                            _server.Connections.Where(x => x == connection).First().User = JsonConvert.DeserializeObject<User>(package.Data);
                            ShowMessage(String.Format("user {0} was signed in", JsonConvert.DeserializeObject<User>(package.Data).Account));
                            ShowInformation();
                            break;
                        /// =============================
                        /// 使用者登出
                        /// =============================
                        case SocketPackage.EnumAction.ClientSignout:
                            _server.Connections.RemoveAll(x => x.User.Account == package.Data);
                            ShowMessage(String.Format("user {0} was signed out", package.Data));
                            ShowInformation();
                            break;
                        /// =============================
                        /// 傳遞訊息
                        /// =============================
                        case SocketPackage.EnumAction.ClientSendMessage:
                            Message message = JsonConvert.DeserializeObject<Message>(package.Data);
                            SocketConnection target = _server.Connections.Where(x => x.User.Account == message.To.Account).FirstOrDefault();
                            if (target != null)
                            {
                                target.Send(new SocketPackage()
                                {
                                    Action = SocketPackage.EnumAction.ClientReciveMessage,
                                    Data = package.Data
                                });
                                ShowMessage(String.Format("user {0} → user {1} (success): {2}", message.From.Name, message.To.Name, message.Content));
                            }
                            else
                            {
                                ShowMessage(String.Format("user {0} → user {1} (failed): {2}", message.From.Name, message.To.Name, message.Content));
                            }
                            break;
                    }
                }));
            }
        }
        #endregion
    }
}
