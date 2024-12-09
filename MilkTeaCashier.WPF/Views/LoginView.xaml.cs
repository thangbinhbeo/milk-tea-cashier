using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.OrderView;
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
using System.Windows.Shapes;

namespace MilkTeaCashier.WPF.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter full login information!!!", "Notifications", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
         
                var employeeService = new EmployeeService(new GenericRepository<Employee>());

             
                var employee = await employeeService.AuthenticateAsync(username, password);
                if (employee.Status == "Inactive") 
                {
                    MessageBox.Show("Account is not active!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (employee.Role == "Manager")
                {
                    MessageBox.Show($"Login successfully! Hello, {employee.FullName} (Manager).", "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);

                    var managerView = new ManagerView();
                    managerView.Show();
                }
                else if (employee.Role == "Cashier")
                {
                    MessageBox.Show($"Login successfully! Hello, {employee.FullName} (Cashier).", "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);
                     
                    var cashierView = new OrderView.Index(); 
                    cashierView.Show();
                }
                else
                {
                    MessageBox.Show("Invalid access!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Close();
            }
            catch (UnauthorizedAccessException)
            {

                MessageBox.Show("Incorrect username or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
