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
    /// <summary>
    /// ChattingRoomSingle.xaml 的互動邏輯
    /// </summary>
    public partial class ChattingRoomSingle : Window
    {
        private User _friend;

        public ChattingRoomSingle(User friend)
        {
            InitializeComponent();
            _friend = friend;
            this.Title = _friend.Name;
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = new Border();
            border.CornerRadius = new CornerRadius(5);
            border.Padding = new Thickness(10);
            border.Background = Brushes.Red;

            TextBlock txt = new TextBlock();
            txt.Text = txtMessage.Text;
            
            txt.HorizontalAlignment = HorizontalAlignment.Right;
            
            pnlHistory.Children.Add(border);
            txtMessage.Text = string.Empty;
        }
    }
}
