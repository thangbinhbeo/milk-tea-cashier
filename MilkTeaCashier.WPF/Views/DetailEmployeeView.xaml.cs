using MilkTeaCashier.Data.Models;
using System;
using System.Windows;

namespace MilkTeaCashier.WPF.Views
{
 
    public partial class DetailEmployeeView : Window
    {
        public DetailEmployeeView(Employee employee)
        {
            InitializeComponent();

       
            txtUsername.Text = employee.Username;
            txtFullName.Text = employee.FullName;
            txtRole.Text = employee.Role;
            txtStatus.Text = employee.Status;
            txtCreatedAt.Text = employee.CreatedAt?.ToString("g"); 
            txtUpdatedAt.Text = employee.UpdatedAt?.ToString("g"); 
        }      

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
