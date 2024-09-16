using SeptemberWPF;
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
using SeptemberWPF.Model;
namespace SeptemberWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private MainWindow mW;

        public Login(MainWindow mainWindow)
        {
            InitializeComponent();
            mW = mainWindow;
        }

        private void LogiClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string usern = txtUsername.Text;
                string pass = txtPassword.Password;

                var user = AppData.db.Users.FirstOrDefault(u => u.Username == usern && u.PasswordHash == pass);

                if (user != null)
                {
                    MessageBox.Show($"Добро пожаловать, {user.FullName}! Ваша роль: {user.Role}");

                    switch (user.Role)
                    {
                        case "Администратор":
                            mW.MainFrame.Navigate(new Admin());
                            break;
                        case "Официант":
                            mW.MainFrame.Navigate(new Wai(user.UserID));
                            break;
                        case "Повар":
                            mW.MainFrame.Navigate(new Coockp(user.UserID));
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}   