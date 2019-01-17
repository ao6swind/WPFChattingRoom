using Client.Pages;
using Models;
using Modules;
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
            _client = new SocketClient();
            _client.OnSocketClientConnectFailed += _client_OnSocketClientConnectFailed;
            _client.OnSocketClientConnectSuccess += _client_OnSocketClientConnectSuccess;
            _client.Connect(user);
        }
        #endregion

        #region [方法][登出]
        public void Disconnection()
        {
            _client.Disonnect();
            pnlFrame.Navigate(new PageLogin());
        }
        #endregion

        // ===============================================
        // 自定義事件處理函式
        // ===============================================

        #region [SocketClient][連線成功時]
        private void _client_OnSocketClientConnectSuccess(SocketClient sender)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                pnlFrame.Navigate(new PageLobby(_user));
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
