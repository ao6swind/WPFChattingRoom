using Client.Pages;
using Models;
using Modules.SocketModules;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Client
{
    public partial class MainWindow : Window
    {
        #region [欄位]
        private SocketClient _client;
        private User _user;
        #endregion

        #region [建構式]
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        // ===============================================
        // 公開方法，為了讓frame裡面的page來呼叫
        // ===============================================

        #region [方法][連線至伺服器]
        public void Connection(User user)
        {
            _user = user;
            _client = new SocketClient(user);
            _client.OnSocketClientCloseSuccess += _client_OnSocketClientCloseSuccess;
            _client.OnSocketClientReceivedSuccess += _client_OnSocketClientReceivedSuccess;
            _client.Connect();
        }
        #endregion

        #region [方法][登出]
        public void Disconnection()
        {
            _client.Send(new SocketPackage()
            {
                Action = SocketPackage.EnumAction.ClientSignout,
                Data = _user.Account
            });
            _client.Disonnect();
            pnlFrame.Navigate(new PageSignin());
        }
        #endregion

        #region [方法][傳送訊息]
        public void Send(User friend, string content)
        {
            _client.Send(new SocketPackage()
            {
                Action = SocketPackage.EnumAction.ClientSendMessage,
                Data = JsonConvert.SerializeObject(new Message()
                {
                    From = _user,
                    To = friend,
                    Content = content
                })
            });
        }
        #endregion

        // ===============================================
        // 自定義事件處理函式
        // ===============================================

        #region [SocketClient][收到資料時]
        private void _client_OnSocketClientReceivedSuccess(SocketClient sender, byte[] data)
        {
            if (data.Length > 0)
            {
                SocketPackage package = SocketPackage.Unpackage(data);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    switch (package.Action)
                    {
                        /// =============================
                        /// 伺服器端要求傳送身分資料
                        /// =============================
                        case SocketPackage.EnumAction.AuthenticationRequest:
                            _client.Send(new SocketPackage()
                            {
                                Action = SocketPackage.EnumAction.AuthenticationResponse,
                                Data = JsonConvert.SerializeObject(_user)
                            });
                            pnlFrame.Navigate(new PageLobby(sender.User));
                            break;
                        /// =============================
                        /// 伺服器端傳來訊息
                        /// =============================
                        case SocketPackage.EnumAction.ClientReciveMessage:
                            Message message = JsonConvert.DeserializeObject<Message>(package.Data);
                            ChattingRoomSingle window = Application.Current.Windows.OfType<ChattingRoomSingle>().Where(x => x.FriendAccount == message.From.Account).FirstOrDefault();
                            if (window != null)
                            {
                                window.pnlHistory.Children.Add(new TextBlock()
                                {
                                    Text = message.Content,
                                    Padding = new Thickness(10),
                                    Margin = new Thickness(10),
                                    Background = Brushes.Blue,
                                    Foreground = Brushes.White,
                                    HorizontalAlignment = HorizontalAlignment.Left
                                });
                                window.pnlScroller.ScrollToEnd();
                                window.Focus();
                            }
                            else
                            {
                                ChattingRoomSingle room = new ChattingRoomSingle(message.From);
                                room.pnlHistory.Children.Add(new TextBlock()
                                {
                                    Text = message.Content,
                                    Padding = new Thickness(10),
                                    Margin = new Thickness(10),
                                    Background = Brushes.Blue,
                                    Foreground = Brushes.White,
                                    HorizontalAlignment = HorizontalAlignment.Left
                                });
                                room.pnlScroller.ScrollToEnd();
                                room.Show();
                            }
                            break;
                        /// =============================
                        /// 伺服器端傳來廣播
                        /// =============================
                        case SocketPackage.EnumAction.ServerBroadcast:
                            foreach (ChattingRoomSingle chattingRoom in Application.Current.Windows.OfType<ChattingRoomSingle>().ToList())
                            {
                                chattingRoom.pnlHistory.Children.Add(new TextBlock()
                                {
                                    Text = String.Format("System message: {0}", package.Data),
                                    Padding = new Thickness(10),
                                    Margin = new Thickness(10),
                                    Background = Brushes.Black,
                                    Foreground = Brushes.White,
                                    HorizontalAlignment = HorizontalAlignment.Center
                                });
                                chattingRoom.pnlScroller.ScrollToEnd();
                            }
                            break;
                        /// =============================
                        /// 伺服器端關閉
                        /// =============================
                        case SocketPackage.EnumAction.ServerShutdown:
                            
                            _client.Disonnect();
                            pnlFrame.Navigate(new PageSignin());
                            break;
                    }
                }));
            }
        }
        #endregion

        #region [SocketClient][關閉成功時]
        private void _client_OnSocketClientCloseSuccess(SocketClient sender)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlFrame.Navigate(new PageLobby(sender.User));
            }));
        }
        #endregion

        #region [SocketClient][連線失敗時]
        private void _client_OnSocketClientConnectFailed(SocketClient sender, string msg)
        {
            MessageBox.Show(msg);
        }
        #endregion
    }
}
