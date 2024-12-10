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
    /// Interaction logic for NewCategory.xaml
    /// </summary>
    public partial class NewCategory : Window
    {
        private readonly CategoryService _service;
        public NewCategory()
        {
            InitializeComponent();
            _service ??= new CategoryService();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            var category = new CategoryManagement();
            category.ShowDialog();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var categoryName = Name.Text;
            var description = Description.Text;

            if (string.IsNullOrWhiteSpace(categoryName) || string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }
            var newCategory = new CreateNewCategory
            {
                CategoryName = categoryName,
                Description = description
            };
            var result = await _service.CreateNewCategory(newCategory);

            MessageBox.Show(result);

            this.Close();

            var category = new CategoryManagement();
            category.ShowDialog();
        }
    }
}
