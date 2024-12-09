using MilkTeaCashier.Data.Models;
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
    /// Interaction logic for ManagerView.xaml
    /// </summary>
    public partial class ManagerView : Window
    {
        public ManagerView()
        {
            InitializeComponent();
        }

        private void ManageEmployees_Click(object sender, RoutedEventArgs e)
        {
            var employeeView = new EmployeeView();
            employeeView.Show();
        }
        
        private void ManageCustomers_Click(object sender, RoutedEventArgs e)
        {
            var customerView = new CustomerInfoWindow();
            customerView.Show();
        }

        private void ManageProducts_Click(object sender, RoutedEventArgs e)
        {
            var productView = new ProductView();
            productView.Show();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            var reportView = new ReportView();
            reportView.Show();
            
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginView();
            login.Show();
            this.Close();
        }
    }
}
