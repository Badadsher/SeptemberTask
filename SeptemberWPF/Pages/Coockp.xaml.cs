using SeptemberWPF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для Coockp.xaml
    /// </summary>
    public partial class Coockp : Page
    {
        private int currentU;
        public Coockp(int userId)
        {
            InitializeComponent();
            currentU = userId;
            LoadOrders();
            LoadWaiters();
        }

        private void LoadOrders()
        {
            var orders = AppData.db.Orders
                .Select(o => new
                {
                    o.OrderID,
                    WaiterName = AppData.db.Users
                        .Where(u => u.UserID == o.WaiterID)
                        .Select(u => u.FullName)
                        .FirstOrDefault(),
                    o.OrderDate,
                    o.OrderStatus,
                    o.TotalAmount
                })
                .ToList();

            OrdersDataGrid.ItemsSource = orders;
        }

        private void LoadWaiters()
        {
            var waiters = AppData.db.Users
                .Where(u => u.Role == "Официант")
                .ToList();

            WaiterComboBox.ItemsSource = waiters;
            WaiterComboBox.DisplayMemberPath = "FullName";
            WaiterComboBox.SelectedValuePath = "UserID";
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WaiterComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(TotalAmountTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, выберите официанта и введите сумму.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedWaiter = (Users)WaiterComboBox.SelectedItem;
                decimal totalAmount;
                if (!decimal.TryParse(TotalAmountTextBox.Text, out totalAmount))
                {
                    MessageBox.Show("Введите корректную сумму.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newOrder = new Orders
                {
                    WaiterID = selectedWaiter.UserID,
                    OrderStatus = "Готовится",
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.Now
                };

                AppData.db.Orders.Add(newOrder);
                AppData.db.SaveChanges();

                LoadOrders();
                MessageBox.Show("Заказ добавлен успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void ChangeStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = (Button)sender;
                var order = (dynamic)button.DataContext;

                if (order == null)
                {
                    MessageBox.Show("Не удалось найти заказ.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var orderID = (int)order.OrderID;
                var existingOrder = AppData.db.Orders.FirstOrDefault(o => o.OrderID == orderID);
                if (existingOrder == null)
                {
                    MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (existingOrder.OrderStatus == "Готовится")
                {
                    existingOrder.OrderStatus = "Готов";
                    AppData.db.Entry(existingOrder).State = EntityState.Modified;
                    AppData.db.SaveChanges();

                    LoadOrders();
                    MessageBox.Show("Статус заказа обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Заказ уже готов или его статус не может быть изменен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
