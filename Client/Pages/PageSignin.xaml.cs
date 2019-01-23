using Models;
using Models.DbContexts;
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

namespace Client.Pages
{
    public partial class PageSignin : Page
    {
        public PageSignin()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (txtAccount.Text != string.Empty && txtPassword.Password != string.Empty)
            {
                ChattingRoomEntities db = new ChattingRoomEntities();
                string password = Auth.Hash(txtPassword.Password);
                User user = db.Users.FirstOrDefault(x =>
                    x.Account == txtAccount.Text &&
                    x.Password == password
                );

                if (user == null)
                {
                    MessageBox.Show("Can not find this user.");
                }
                else
                {
                    ((MainWindow)Application.Current.MainWindow).Connection(user);
                }
            }
            else
            {
                MessageBox.Show("Account or password is empty");
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Application.Current.MainWindow.FindName("pnlFrame")).Navigate(new PageRegist());
        }
    }
}
