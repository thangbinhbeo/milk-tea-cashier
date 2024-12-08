using MilkTeaCashier.Data.DTOs;
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
    /// Interaction logic for CategoryManagement.xaml
    /// </summary>
    public partial class CategoryManagement : Window
    {
        private readonly CategoryService _service;
        public CategoryManagement()
        {
            InitializeComponent();
            _service = new CategoryService();
            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _service.GetAllCategory();
                if (categories != null)
                {
                    CategoryDataGrid.ItemsSource = categories;
                }
                else
                {
                    MessageBox.Show("No categories found!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CategoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CategoryDataGrid.SelectedItem != null)
            {
                var selectedItem = CategoryDataGrid.SelectedItem;
                this.Close();

                var detailsWindow = new CategoryDetails(selectedItem);
                detailsWindow.ShowDialog();
            }
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            var newCategory = new NewCategory();
            newCategory.ShowDialog();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
