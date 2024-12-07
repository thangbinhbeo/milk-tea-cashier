using MilkTeaCashier.Data.Repository;
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
	/// Interaction logic for ProductView.xaml
	/// </summary>
	public partial class ProductView : Window
	{
		private readonly ProductService _service;

		public ProductView()
		{
			InitializeComponent();
			_service = new ProductService();
		}

		private async void AddProduct_Click(object sender, RoutedEventArgs e)
		{
			var productDetailView = new ProductDetailView();
			productDetailView.ShowDialog(); // Show the product detail view for adding a new product
		}

		private async void EditProduct_Click(object sender, RoutedEventArgs e)
		{
			var selectedProduct = ProductsDataGrid.SelectedItem as Product;
			if (selectedProduct != null)
			{
				var productDetailView = new ProductDetailView
				{
					EditProduct = selectedProduct // Pass the selected product to the detail view for editing
				};
				productDetailView.ShowDialog();
			}
		}

		private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
		{
			var selectedProduct = ProductsDataGrid.SelectedItem as Product;
			if (selectedProduct != null)
			{
				var result = MessageBox.Show("Are you sure you want to delete this product?", "Delete Product", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					await _service.DeleteProductAsync(selectedProduct.ProductId);
					LoadProducts(); // Refresh the data grid after deletion
				}
			}
		}

		private async void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			var searchText = SearchTextBox.Text;
			var products = await _service.SearchProductsAsync(searchText);
			ProductsDataGrid.ItemsSource = products;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadProducts();
		}

		private async void LoadProducts()
		{
			var products = await _service.GetAllProductsAsync();
			ProductsDataGrid.ItemsSource = products;
		}
	}
}
