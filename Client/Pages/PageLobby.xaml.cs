using Client.UserControls;
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

namespace Client.Pages
{
    /// <summary>
    /// PageLobby.xaml 的互動邏輯
    /// </summary>
    public partial class PageLobby : Page
    {
        public PageLobby()
        {
            InitializeComponent();

            List<User> friends = new List<User>();

            for (int i = 0; i <= 5; i++)
            {
                friends.Add(new User()
                {
                    Id = i + 1,
                    Name = "使用者" + i.ToString(),
                    Account = "Account" + i.ToString(),
                    Password = "Password" + i.ToString()
                });
            }
            int number = 0;
            foreach(User friend in friends)
            {
                number++;
                pnlFriends.Children.Add(new FriendInfo(friend, ((number % 8) + 1)));
            }
        }
    }
}
