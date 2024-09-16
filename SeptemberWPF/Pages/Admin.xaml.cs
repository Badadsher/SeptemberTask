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
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Page
    {
        public Admin()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            EmployeeGrid.ItemsSource = AppData.db.Users.ToList();

            var ordersData = from order in AppData.db.Orders
                             join waiter in AppData.db.Users on order.WaiterID equals waiter.UserID
                             select new
                             {
                                 order.OrderID,
                                 WaiterName = waiter.FullName,
                                 order.OrderDate,
                                 order.OrderStatus,
                                 order.TotalAmount
                             };
            OrdersGrid.ItemsSource = ordersData.ToList();

            var shiftsData = from shift in AppData.db.Smena
                             join waiter in AppData.db.Users on shift.WaiterID equals waiter.UserID
                             join cook in AppData.db.Users on shift.CookID equals cook.UserID
                             select new
                             {
                                 shift.SmenaID,
                                 shift.SmenaDate,
                                 WaiterName = waiter.FullName,
                                 CookName = cook.FullName
                             };

            ShiftsGrid.ItemsSource = shiftsData.ToList();
        }

        private void AddEmp(object sender, RoutedEventArgs e)
        {
            try
            {
                var addEmployeePage = new AddEmp(LoadData);
                NavigationService.Navigate(addEmployeePage);
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void FireEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEmployee = (Users)EmployeeGrid.SelectedItem;
                if (selectedEmployee != null && selectedEmployee.Status != "Уволен")
                {
                    selectedEmployee.Status = "Уволен";
                    AppData.db.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Пользователь уже уволен или не выбран", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ReinstateEmp(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedEmployee = (Users)EmployeeGrid.SelectedItem;
                if (selectedEmployee != null && selectedEmployee.Status == "Уволен")
                {
                    selectedEmployee.Status = "Активен";
                    AppData.db.SaveChanges();
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Выбранный пользователь не уволен или не выбран", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void AssignShif(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AsignSmena());
        }

     
    }
}
