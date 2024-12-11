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
            dpCreatedAt.Text = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password;
                string fullName = txtFullName.Text.Trim();
                var selectedRole = cmbRole.SelectedItem as ComboBoxItem;
                var selectedStatus = "Active";
                DateTime createdAt = DateTime.Now;
                DateTime updatedAt = DateTime.Now;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName) || selectedStatus == null)
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
                    Status = selectedStatus,
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
