using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.Repository;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for EmployeeView.xaml
    /// </summary>
    public partial class EmployeeView : Window
    {

        private readonly EmployeeService _employeeService;
        

        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<OrderDetail> OrderDetails { get; set; }

        public EmployeeView()
        {
            InitializeComponent();

            _employeeService = new EmployeeService();

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
    
                var employees = await _employeeService.GetAllEmployeesAsync();

                Employees = new ObservableCollection<Employee>(employees);
                dgair.ItemsSource = Employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = new CreateEmployeeView();
            var result = createWindow.ShowDialog();

            if (result == true)
            {
                var newEmployee = createWindow.NewEmployee;

                try
                {
                    // Thêm nhân viên mới (dùng await để chờ xử lý xong)
                    await _employeeService.AddEmployeeAsync(newEmployee);

                    MessageBox.Show("Create new employee successfully!", "Announcement", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
           
            var selectedEmployee = dgair.SelectedItem as Employee;

            if (selectedEmployee != null)
            {
         
                var updateWindow = new UpdateEmployeeView(selectedEmployee);
                var result = updateWindow.ShowDialog();

                if (result == true)  
                {
                    var updatedEmployee = updateWindow.UpdateEmployee; 

                    try
                    {
                        
                        await _employeeService.UpdateEmployeeAsync(updatedEmployee);

                       
                        MessageBox.Show("Update employee successfully!", "Announcement", MessageBoxButton.OK, MessageBoxImage.Information);

                    
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                       
                        MessageBox.Show($"Error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to update.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee= dgair.SelectedItem as Employee;

            if (selectedEmployee != null)
            {
                var messageBoxResult = MessageBox.Show("Are you sure want to delete this employee", "Comfirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    _employeeService.DeleteEmployeeAsync(selectedEmployee.EmployeeId);
                    MessageBox.Show("Delete employee successfully!!!", "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    LoadData();
                    
                }
            }
            else
            {
                MessageBox.Show("Please choose employee to delete.", "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            var selectedEmployee = dgair.SelectedItem as Employee;

            if (selectedEmployee != null)
            {
                
                var viewWindow = new DetailEmployeeView(selectedEmployee);
                viewWindow.ShowDialog();
            }
            else
            {
                
                MessageBox.Show("Please select an employee to view.", "Notifications", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
