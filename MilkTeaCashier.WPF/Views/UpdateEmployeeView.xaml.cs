using MilkTeaCashier.Data.Models;
using System.Windows.Controls;
using System.Windows;

namespace MilkTeaCashier.WPF.Views
{
    public partial class UpdateEmployeeView : Window
    {
        public Employee UpdateEmployee { get; private set; }

        public UpdateEmployeeView(Employee employee)
        {
            InitializeComponent();
  
            UpdateEmployee = employee;
    
            txtUsername.Text = UpdateEmployee.Username;
            txtPassword.Password = UpdateEmployee.PasswordHash; 
            txtFullName.Text = UpdateEmployee.FullName;
            cmbRole.SelectedValue = UpdateEmployee.Role;
            cmbStatus.SelectedValue = UpdateEmployee.Status;
            dpCreatedAt.SelectedDate = UpdateEmployee.CreatedAt;
            dpUpdatedAt.SelectedDate = UpdateEmployee.UpdatedAt;
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

           
                UpdateEmployee.Username = username;
                UpdateEmployee.PasswordHash = password;  
                UpdateEmployee.FullName = fullName;
                UpdateEmployee.Role = selectedRole.Tag.ToString();
                UpdateEmployee.Status = selectedStatus.Tag.ToString();
                UpdateEmployee.CreatedAt = createdAt;
                UpdateEmployee.UpdatedAt = updatedAt;

                
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
