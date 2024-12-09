using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace MilkTeaCashier.WPF.Views
{
    public partial class CreateCustomerWindow : Window
    {
        private readonly CustomerService _customerService;
        public Customer CurrentCustomer { get; private set; }
        private bool isEditMode;

        public CreateCustomerWindow(Customer customer = null)
        {
            InitializeComponent();
            _customerService ??= new CustomerService();

            if (customer != null)
            {
                CurrentCustomer = customer;
                isEditMode = true;
                LoadCustomerData();
            }
            else
            {
                CurrentCustomer = new Customer();
                isEditMode = false;
            }
        }

        private void LoadCustomerData()
        {
            dgName.Text = CurrentCustomer.Name;
            dgPhone.Text = CurrentCustomer.Phone;
            dgGender.SelectedItem = dgGender.Items.Cast<ComboBoxItem>()
                .FirstOrDefault(item => (string)item.Content == CurrentCustomer.Gender);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = dgName.Text.Trim();
            string phone = dgPhone.Text.Trim();
            string gender = (dgGender.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (string.IsNullOrWhiteSpace(name) )
            {
                MessageBox.Show("Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string pattern = @"^[a-zA-Z\s]+$"; // Only allows letters and spaces
            if (!Regex.IsMatch(name, pattern))
            {
                MessageBox.Show("Name cannot contain special characters and numbers.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                if (isEditMode)
                {
                    CurrentCustomer.Name = name;
                    CurrentCustomer.Phone = phone;
                    CurrentCustomer.Gender = gender;

                    // Call the update method
                    await _customerService.UpdateCustomerAsync(CurrentCustomer.CustomerId, name, phone, gender);
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Tag = CurrentCustomer;
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