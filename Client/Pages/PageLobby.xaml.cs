using Client.UserControls;
using Models;
using Models.DbContexts;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.Pages
{
    public partial class PageLobby : Page
    {
        private ChattingRoomEntities db = new ChattingRoomEntities();
        private User user;
        public PageLobby(User _user)
        {
            InitializeComponent();
            user = _user;
            ShowFriends(user);
        }

        private void ShowFriends(User user)
        {
            pnlFriends.Children.Clear();
            List<Friend> friends = db.Friends.Where(x => x.UserId == user.Id).ToList();
            int number = 0;
            foreach (Friend friend in friends)
            {
                User myFriend = db.Users.Where(x => x.Id == friend.FriendId).First();
                pnlFriends.Children.Add(new FriendInfo(myFriend, ((number % 8) + 1)));
            }
        }

        private void BtnLogout_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<ChattingRoomSingle> windows = Application.Current.Windows.OfType<ChattingRoomSingle>().ToList();
            foreach (ChattingRoomSingle window in windows)
            {
                window.Close();
            }
            ((MainWindow)Application.Current.MainWindow).Disconnection();
        }

        private void BtnAddFriend_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (db.Users.Any(x => x.Account == txtFriendAccount.Text))
            {
                User friend = db.Users.Where(x => x.Account == txtFriendAccount.Text).First();
                db.Friends.Add(new Friend()
                {
                    UserId = user.Id,
                    FriendId = friend.Id
                });
                db.SaveChanges();
                ShowFriends(user);
            }
        }
    }
}
