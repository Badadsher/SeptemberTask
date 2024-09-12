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
    /// Логика взаимодействия для Wai.xaml
    /// </summary>
    public partial class Wai : Page
    {
        private int _currentUserId;
        public Wai(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
            LoadOrderItems();
        }

        private void LoadOrderItems()
        {
            var orderItems = AppData.db.OrderItems
                .Join(AppData.db.Orders, oi => oi.OrderID, o => o.OrderID, (oi, o) => new { oi, o })
                .Where(joined => joined.o.WaiterID == _currentUserId)
                .Select(joined => joined.oi)
                .ToList();
            OrderItemsDataGrid.ItemsSource = orderItems;
        }

        private void AddOrderItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(QuantityTextBox.Text, out int quantity) &&
                    decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    var order = AppData.db.Orders
                        .FirstOrDefault(o => o.WaiterID == _currentUserId && o.OrderStatus == "Принят");

                    if (order == null)
                    {
                        order = new Orders
                        {
                            WaiterID = _currentUserId,
                            OrderStatus = "Принят",
                            TotalAmount = 0,
                            OrderDate = DateTime.Now
                        };

                        AppData.db.Orders.Add(order);
                        AppData.db.SaveChanges();
                    }

                    var orderItem = new OrderItems
                    {
                        OrderID = order.OrderID,
                        MenuItem = MenuItemTextBox.Text,
                        Quantity = quantity,
                        Price = price
                    };

                    AppData.db.OrderItems.Add(orderItem);
                    order.TotalAmount += price * quantity;
                    AppData.db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    AppData.db.SaveChanges();

                    LoadOrderItems();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, заполните все поля правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                var newOrder = new Orders
                {
                    WaiterID = _currentUserId,
                    OrderStatus = "Готовится",
                    OrderDate = DateTime.Now,
                    TotalAmount = Convert.ToDecimal(PriceTextBox.Text)
                };

                AppData.db.Orders.Add(newOrder);
                AppData.db.SaveChanges();
                LoadOrderItems();
                MessageBox.Show("Заказ создан успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }
    }
}
