using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
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
    public partial class CreateCustomerWindow : Window
    {
        private readonly CustomerService _customerService;
        public Customer CurrentCustomer { get; private set; }
        private bool isEditMode;

        public CreateCustomerWindow(CustomerService customerService, Customer customer = null)
        {
            InitializeComponent();
            _customerService = customerService;

            if (customer != null)
            {
                CurrentCustomer = customer;
                isEditMode = true;
                LoadCustomerData();
            }
            else
            {
                CurrentCustomer = new Customer();
            }
        }

        private void LoadCustomerData()
        {
            dgName.Text = CurrentCustomer.Name;
            dgPhone.Text = CurrentCustomer.Phone;
            if (CurrentCustomer.Gender == "Male")
            {
                dgGender.SelectedItem = dgGender.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string)item.Content == "Male");
            }
            else if (CurrentCustomer.Gender == "Female")
            {
                dgGender.SelectedItem = dgGender.Items.Cast<ComboBoxItem>().FirstOrDefault(item => (string)item.Content == "Female");
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = dgName.Text.Trim();
            string phone = dgPhone.Text.Trim();
            string gender = (dgGender.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10 || !phone.StartsWith("0"))
            {
                MessageBox.Show("Phone number must start with 0 and be 10 digits long.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(gender))
            {
                MessageBox.Show("Gender must be selected.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var customerDto = new CreateCustomerDto
            {
                Name = name,
                Phone = phone,
                Gender = gender,
            };

            try
            {
                if (isEditMode) // Check if editing an existing customer
                {
                    CurrentCustomer.Name = name;
                    CurrentCustomer.Phone = phone;
                    CurrentCustomer.Gender = gender;

                    // Call the update method
                    await _customerService.UpdateCustomerAsync(CurrentCustomer.CustomerId, name, phone, gender);
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else 
                {
                    Customer newCustomer = await _customerService.AddCustomerAsync(customerDto);
                    MessageBox.Show("New customer created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Tag = newCustomer; 
                }

                this.DialogResult = true; 
                Close(); 
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}