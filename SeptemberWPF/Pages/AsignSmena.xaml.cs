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
    /// Логика взаимодействия для AsignSmena.xaml
    /// </summary>
    public partial class AsignSmena : Page
    {
        public AsignSmena()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            using (var db = new NasyrovTaskSeptemberEntities())
            {
                var employees = db.Users.Where(u => u.Status == "Активен").ToList();
                WaiterComboBox.ItemsSource = employees;
                CookComboBox.ItemsSource = employees;
            }
        }

        private void AssignShift_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (WaiterComboBox.SelectedItem == null || CookComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, выберите и официанта, и повара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedWaiter = (Users)WaiterComboBox.SelectedItem;
                var selectedCook = (Users)CookComboBox.SelectedItem;
                var shiftDate = ShiftDatePicker.SelectedDate;

                if (shiftDate == null)
                {
                    MessageBox.Show("Пожалуйста, выберите дату смены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var db = new NasyrovTaskSeptemberEntities())
                {
                    var newShift = new Smena
                    {
                        SmenaDate = shiftDate.Value,
                        WaiterID = selectedWaiter.UserID,
                        CookID = selectedCook.UserID
                    };

                    db.Smena.Add(newShift);
                    db.SaveChanges();

                    MessageBox.Show("Смена назначена успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.Navigate(new Admin());
                }
            }
            catch
            {
                MessageBox.Show("Ошибка");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
