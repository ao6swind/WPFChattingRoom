using Models;
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
using System.Windows.Shapes;

namespace Client
{
    public partial class ChattingRoomSingle : Window
    {
        #region [欄位]
        private User _friend;
        #endregion

        #region [屬性]
        public string FriendAccount { get { return _friend.Account; } }
        #endregion

        #region [建構式]
        public ChattingRoomSingle(User friend)
        {
            InitializeComponent();
 
            Title = friend.Name;
            _friend = friend;
        }
        #endregion

        // ===============================================
        // UI事件處理函式
        // ===============================================

        #region [按鈕][左鍵][傳送訊息]
        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtMessage.Text != String.Empty)
            {
                ((MainWindow)(Application.Current.MainWindow)).Send(_friend, txtMessage.Text);

                TextBlock txt = new TextBlock();
                txt.Text = txtMessage.Text;
                txt.Padding = new Thickness(10);
                txt.Margin = new Thickness(10);
                txt.Background = Brushes.Purple;
                txt.Foreground = Brushes.White;
                txt.HorizontalAlignment = HorizontalAlignment.Right;

                pnlHistory.Children.Add(txt);
                txtMessage.Text = String.Empty;
            }
        }
        #endregion
    }
}
