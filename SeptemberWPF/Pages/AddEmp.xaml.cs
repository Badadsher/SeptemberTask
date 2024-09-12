using SeptemberWPF.Model;
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

namespace SeptemberWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEmp.xaml
    /// </summary>
    public partial class AddEmp : Page
    {
        private Action _refreshEmployeeTable;
        public AddEmp(Action refreshEmployeeTable)
        {
            InitializeComponent();
            _refreshEmployeeTable = refreshEmployeeTable;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string role = ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString();

            try
            {
                if (!string.IsNullOrEmpty(fullName) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    var newUser = new Users
                    {
                        FullName = fullName,
                        Username = username,
                        PasswordHash = password,
                        Role = role,
                        Status = "Активен"
                    };

                    AppData.db.Users.Add(newUser);
                    AppData.db.SaveChanges();

                    MessageBox.Show("Новый сотрудник добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    _refreshEmployeeTable?.Invoke();

                    NavigationService.GoBack();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
    