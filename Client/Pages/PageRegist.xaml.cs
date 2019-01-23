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
    public partial class PageRegist : Page
    {
        public PageRegist()
        {
            InitializeComponent();
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChattingRoomEntities db = new ChattingRoomEntities();
            if (db.Users.Any(x => x.Account == txtAccount.Text))
            {
                MessageBox.Show("This account is existed.");
            }
            else
            {
                db.Users.Add(new User()
                {
                    Name = txtName.Text,
                    Account = txtAccount.Text,
                    Password = Auth.Hash(txtPassword.Password)
                });
                db.SaveChanges();
                MessageBox.Show("Regist success! Please try to login your account.");
                ((Frame)Application.Current.MainWindow.FindName("pnlFrame")).Navigate(new PageSignin());
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Application.Current.MainWindow.FindName("pnlFrame")).Navigate(new PageSignin());
        }
    }
}
