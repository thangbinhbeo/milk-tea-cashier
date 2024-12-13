using MilkTeaCashier.Service.Interfaces;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MilkTeaCashier.WPF.Views
{
    public partial class CustomerInfoWindow : Window
    {
        private readonly CustomerService _customerService;
        private readonly ObservableCollection<Customer> _customers;

        private int _employeeID;

        public CustomerInfoWindow(int employeeID)
        {
            InitializeComponent();
            _customerService = new CustomerService();
            _customers = new ObservableCollection<Customer>();
            LoadCustomersAsync();
            _employeeID = employeeID;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                dgCustomer.ItemsSource = customers; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCustomerWindow addCustomerWindow = new CreateCustomerWindow(_employeeID);
            bool? result = addCustomerWindow.ShowDialog();

            if (result == true && addCustomerWindow.Tag is Customer newCustomer)
            {
                _customers.Add(newCustomer);
                LoadCustomersAsync();
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem is Customer selectedCustomer)
            {
                Debug.WriteLine($"Selected customer Id {selectedCustomer.CustomerId} and customer name to update: {selectedCustomer.Name}");
                CreateCustomerWindow editCustomerWindow = new CreateCustomerWindow(selectedCustomer, _employeeID);
                var result = editCustomerWindow.ShowDialog();

                if (result == true)
                {
                    await LoadCustomersAsync(); 
                    var updatedCustomer = _customers.FirstOrDefault(c => c.CustomerId == selectedCustomer.CustomerId);
                    Debug.WriteLine($"UpdatedBy after refresh: {updatedCustomer?.UpdatedBy}");

                    dgCustomer.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to update.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem is Customer customer)
            {
                var result = MessageBox.Show("Are you sure you want to delete this customer?", "Delete Customer", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _customerService.DeleteCustomerAsync(customer.CustomerId);
                        await LoadCustomersAsync();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message, "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting customer: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string name = txtCustomerName.Text.Trim();
            string phone = txtPhone.Text.Trim();

            try
            {
                var customers = await _customerService.SearchCustomerByNameAndPhoneAsync(name, phone);
                _customers.Clear(); 
                foreach (var customer in customers)
                {
                    _customers.Add(customer);
                }
                dgCustomer.ItemsSource = _customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching customers: {ex.Message}", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}