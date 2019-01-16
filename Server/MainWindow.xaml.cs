using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Models;
namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        #region [欄位]
        private SocketServer _server;
        #endregion

        #region [建構式]
        public MainWindow()
        {
            InitializeComponent();

            // 建立Socket Server
            _server = new SocketServer();
            _server.OnSocketServerInitSuccess   += _server_OnSocketServerInitSuccess;   // 初始化成功
            _server.OnSocketServerInitFailed    += _server_OnSocketServerInitFailed;    // 初始化失敗
            _server.OnSocketServerAcceptSuccess += _server_OnSocketServerAcceptSuccess; // 連入成功
            _server.OnSocketServerAcceptFailed  += _server_OnSocketServerAcceptFailed;  // 連入失敗
            _server.OnSocketServerReciveSuccess += _server_OnSocketServerReciveSuccess; // 接收資料成功
            _server.OnSocketServerReciveFailed  += _server_OnSocketServerReciveFailed;  // 接收資料失敗
            _server.OnSocketServerCloseSuccess += _server_OnSocketServerCloseSuccess;   // 關閉成功
            _server.OnSocketServerCloseFailed += _server_OnSocketServerCloseFailed;     // 關閉失敗

            // 顯示基本設定          
            txtIpAddress.Content    = SocketSetting.IP;
            txtPort.Content         = SocketSetting.Port;
            txtNumber.Content       = SocketSetting.Number;
        }
        #endregion

        // ===============================================
        // UI事件處理函式
        // ===============================================

        #region [按鈕][左鍵][啟動伺服器]
        private void BtnTurnOn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _server.Start();
            btnTurnOn.IsEnabled = false;
            btnTurnOff.IsEnabled = true;
        }
        #endregion

        #region [按鈕][左鍵][關閉伺服器]
        private void BtnTurnOff_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _server.Stop();
            btnTurnOn.IsEnabled = true;
            btnTurnOff.IsEnabled = false;
        }
        #endregion

        // ===============================================
        // 自定義事件處理函式
        // ===============================================

        #region [SocketServer][初始化][成功]
        private void _server_OnSocketServerInitSuccess(SocketServer server)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlMessage.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0} # 伺服器初始化成功", DateTime.Now.ToString())
                });
            }));
        }
        #endregion

        #region [SocketServer][初始化][失敗]
        private void _server_OnSocketServerInitFailed(SocketServer server, Exception ex)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlMessage.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0} # 伺服器初始化失敗 # {1}", DateTime.Now.ToString(), ex.Message),
                    Foreground = Brushes.Red
                });
            }));
        }
        #endregion

        #region [SocketServer][關閉][成功]
        private void _server_OnSocketServerCloseSuccess(SocketServer server)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlMessage.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0} # 伺服器關閉成功", DateTime.Now.ToString())
                });
            }));
        }
        #endregion

        #region [SocketServer][關閉][失敗]
        private void _server_OnSocketServerCloseFailed(SocketServer server, Exception ex)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlMessage.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0} # 伺服器關閉失敗 # {1}", DateTime.Now.ToString(), ex.Message),
                    Foreground = Brushes.Red
                });
            }));
        }
        #endregion

        #region [SocketServer][接受連線][成功]
        private void _server_OnSocketServerAcceptSuccess(SocketServer server, SocketClient client)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                txtOnlineCounting.Content = server.OnlineMember;
            }));
        }
        #endregion

        #region [SocketServer][接受連線][失敗]
        private void _server_OnSocketServerAcceptFailed(SocketServer server, Exception ex)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlMessage.Children.Add(new TextBlock()
                {
                    Text = String.Format("{0} # 伺服器接收連線失敗 # {1}", DateTime.Now.ToString(), ex.Message),
                    Foreground = Brushes.Red
                });
            }));
        }
        #endregion

        #region [SocketServer][接收資料][成功]
        private void _server_OnSocketServerReciveSuccess(SocketServer server, byte[] data)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region [SocketServer][接收資料][失敗]
        private void _server_OnSocketServerReciveFailed(SocketServer server, Exception ex)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
