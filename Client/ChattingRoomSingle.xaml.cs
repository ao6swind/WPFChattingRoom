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
        public string FriendAccount { get; private set; }
        private User _friend;

        public ChattingRoomSingle(User friend)
        {
            InitializeComponent();
            FriendAccount = friend.Account;
            Title = friend.Name;
            _friend = friend;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtMessage.Text != String.Empty)
            {
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
    }
}
