using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.UnitOfWork;
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
    /// <summary>
    /// Interaction logic for CategoryDetails.xaml
    /// </summary>
    public partial class CategoryDetails : Window
    {
        private bool isEditing = false;
        private readonly CategoryService _service;

        private string originalDescription;
        private string originalUpdatedAt;
        private string originalName;

        public CategoryDetails(object item)
        {
            InitializeComponent();
            DataContext = item;
            _service ??= new CategoryService();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditing)
            {
                SaveCategoryDetails();

                SetEditingMode(false);

                CancelButton.Visibility = Visibility.Collapsed;

                UpdateButton.Content = "Update";
                isEditing = false;
            }
            else
            {
                originalDescription = DescriptionTextBox.Text;
                originalUpdatedAt = UpdatedAtTextBox.Text;
                originalName = NameTextBox.Text;

                SetEditingMode(true);

                CancelButton.Visibility = Visibility.Visible;

                UpdateButton.Content = "Save";
                isEditing = true;
            }
        }

        private void SetEditingMode(bool enable)
        {
            NameTextBox.IsReadOnly = !enable;
            DescriptionTextBox.IsReadOnly = !enable;

            if (enable)
            {
                UpdatedAtTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private async void SaveCategoryDetails()
        {
            var id = int.Parse(IdTextBox.Text);
            var updatedCategory = new CreateNewCategory
            {
                CategoryName = NameTextBox.Text,
                Description = DescriptionTextBox.Text,
            };

            var result = await _service.UpdateCategory(id, updatedCategory);
            MessageBox.Show(result);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DescriptionTextBox.Text = originalDescription;
            UpdatedAtTextBox.Text = originalUpdatedAt;
            NameTextBox.Text = originalName;

            SetEditingMode(false);

            CancelButton.Visibility = Visibility.Collapsed;

            UpdateButton.Content = "Update";
            isEditing = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            var managementWindow = new CategoryManagement();
            managementWindow.Show();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
        "Are you sure want to delete this item ?",
        "Yes !",
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                RemoveCategory();

                this.Close();

                var managementWindow = new CategoryManagement();
                managementWindow.Show();
            }
            else
            {
                return;
            }
        }

        private async void RemoveCategory()
        {
            try
            {
                var id = int.Parse(IdTextBox.Text);

                var result = await _service.DeleteById(id);

                MessageBox.Show(
                    result,
                    "Alert !",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fail to delete: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
