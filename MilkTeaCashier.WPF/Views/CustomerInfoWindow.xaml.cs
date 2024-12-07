using MilkTeaCashier.Service.Interfaces;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace MilkTeaCashier.WPF.Views
{
    public partial class CustomerInfoWindow : Window
    {
        private readonly ICustomerService _customerService;
        private readonly ObservableCollection<Customer> _customers;

        public CustomerInfoWindow()
        {
            InitializeComponent();
            _customerService = new CustomerService(new GenericRepository<Customer>(), new GenericRepository<Employee>());
            _customers = new ObservableCollection<Customer>();
            LoadCustomers();
        }

        private async void LoadCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                _customers.Clear();
                foreach (var customer in customers)
                {
                    _customers.Add(customer);
                }
                dgCustomer.ItemsSource = _customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateCustomerWindow addCustomerWindow = new CreateCustomerWindow((CustomerService)_customerService);
            bool? result = addCustomerWindow.ShowDialog();

            if (result == true)
            {
                if (addCustomerWindow.Tag is Customer newCustomer)
                {
                    _customers.Add(newCustomer); 
                }
            }
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem is Customer selectedCustomer)
            {
                CreateCustomerWindow editCustomerWindow = new CreateCustomerWindow((CustomerService)_customerService, selectedCustomer);
                bool? result = editCustomerWindow.ShowDialog();

                if (result == true)
                {
                    LoadCustomers();
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
                        LoadCustomers(); 
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
                MessageBox.Show($"Error searching customers: {ex.Message}");
            }
        }
    }
}