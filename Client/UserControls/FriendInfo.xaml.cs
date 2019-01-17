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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.UserControls
{
    /// <summary>
    /// FriendInfo.xaml 的互動邏輯
    /// </summary>
    public partial class FriendInfo : UserControl
    {
        private User _friend;

        public FriendInfo(User friend, int image)
        {
            InitializeComponent();
            _friend = friend;
            txtFriendName.Content = friend.Name;
            imgUser.Source = new BitmapImage(new Uri(@"../Resources/Images/Users/"+image.ToString()+".png", UriKind.Relative));
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.Windows.OfType<ChattingRoomSingle>().Any(x => x.FriendAccount == _friend.Account))
            {
                Application.Current.Windows
                    .OfType<ChattingRoomSingle>()
                    .Where(x => x.FriendAccount == _friend.Account)
                    .First()
                    .Focus();
            }
            else
            {
                ChattingRoomSingle room = new ChattingRoomSingle(_friend);
                room.Show();
            }
        }
    }
}
