using System;
using System.Collections.Generic;
using System.Linq;
using MilkTeaCashier.Data.Models;
using System;
using System.Windows;
using Org.BouncyCastle.Asn1.Cmp;
using System.Windows.Controls;

namespace MilkTeaCashier.WPF.Views
{
    public partial class CreateEmployeeView : Window
    {
        public Employee NewEmployee { get; private set; }

        public CreateEmployeeView()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password;
                string fullName = txtFullName.Text.Trim();
                var selectedRole = cmbRole.SelectedItem as ComboBoxItem;
                var selectedStatus = cmbStatus.SelectedItem as ComboBoxItem;
                DateTime? createdAt = dpCreatedAt.SelectedDate;
                DateTime? updatedAt = dpUpdatedAt.SelectedDate;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName) || selectedRole == null || selectedStatus == null || createdAt == null || updatedAt == null)
                {
                    MessageBox.Show("Please fill all fields!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                NewEmployee = new Employee
                {
                    Username = username,
                    PasswordHash = password,
                    FullName = fullName,
                    Role = selectedRole.Tag.ToString(),
                    Status = selectedStatus.Tag.ToString(),
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt
                };

  
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
