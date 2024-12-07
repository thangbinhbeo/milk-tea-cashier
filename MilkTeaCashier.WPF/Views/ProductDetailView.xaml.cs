using MilkTeaCashier.Data.Models;
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
    /// Interaction logic for ProductDetailView.xaml
    /// </summary>
    public partial class ProductDetailView : Window
    {
        private readonly ProductService _service;

        public Product EditProduct { get; set; } = null;
        public ProductDetailView()
        {
            InitializeComponent();
        }

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
            Product product = new Product();
            product.ProductId = int.Parse(ProductIdTextBox.Text);
            product.CategoryId = int.Parse(CategoryIdTextBox.Text);
            product.Name = NameTextBox.Text;
            product.Size = SizeTextBox.Text;
            product.Price = double.Parse(PriceTextBox.Text);
            product.Status = StatusTextBox.Text;

            await _service.AddProductAsync(product);
            this.Close();
		}

        private void Window_Loaded (object sender, RoutedEventArgs e) 
        {
            FillComboBox();
        }

        private void FillElements(Product x)
        {
            if (x == null)
                return;
			ProductIdTextBox.Text = x.ProductId.ToString();
			CategoryIdTextBox.Text = x.CategoryId.ToString();
			NameTextBox.Text = x.Name.ToString();
			SizeTextBox.Text = x.Size.ToString();
			PriceTextBox.Text = x.Price.ToString();
			StatusTextBox.Text = x.Status.ToString();



		}

        private void FillComboBox() 
        {
			CategoryComboBox.ItemsSource = _c

		}
	}
}
