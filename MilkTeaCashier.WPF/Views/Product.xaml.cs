using Microsoft.Win32;
using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : Window
    {
        private string _selectedFilePath;
        private string _fileExtension;
        private readonly ImageUploadService _firebaseService;
        private readonly ProductService _productService;

        public Product()
        {
            InitializeComponent();
            _firebaseService = new ImageUploadService();
            _productService = new ProductService();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
            openFileDialog.Title = "Upload a product photo";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                _fileExtension = System.IO.Path.GetExtension(_selectedFilePath);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_selectedFilePath);
                bitmap.EndInit();

                ProductImage.Source = bitmap;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string name = Name.Text;
            int category = int.Parse(Category.Text);
            string size = Size.Text;
            double price = double.Parse(Price.Text);
            string url = "";

            try
            {
                string fileName = System.IO.Path.GetFileName(_selectedFilePath);
                url = await _firebaseService.UploadImageAsync(_selectedFilePath, _fileExtension);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            var newProduct = new CreateProductModel
            {
                Name = name,
                CategoryId = category,
                Image = url,
                Price = price,
                Size = size,
            };

            var result = await _productService.AddProductAsync(newProduct);
            MessageBox.Show(result);
        }
    }
}
