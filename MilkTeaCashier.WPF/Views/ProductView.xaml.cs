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
			else
			{
				MessageBox.Show("Please select a product to edit.", "Edit Product", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
		{
			var selectedProduct = ProductsDataGrid.SelectedItem as Product;
			if (selectedProduct != null)
			{
				var result = MessageBox.Show("Are you sure you want to delete this product?", "Delete Product", MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result == MessageBoxResult.Yes)
				{
					try
					{
						await _service.DeleteProductAsync(selectedProduct.ProductId);
						LoadProducts(); // Refresh the data grid after deletion
					}
					catch (Exception ex)
					{
						MessageBox.Show($"An error occurred while deleting the product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Please select a product to delete.", "Delete Product", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private async void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			var searchText = SearchTextBox.Text;
			if (!string.IsNullOrWhiteSpace(searchText))
			{
				var products = await _service.SearchProductsAsync(searchText);
				ProductsDataGrid.ItemsSource = products;
			}
			else
			{
				LoadProducts(); // Load all products if search text is empty
			}
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadProducts();
		}

		private async void LoadProducts()
		{
			try
			{
				var products = await _service.GetAllProductsAsync();
				ProductsDataGrid.ItemsSource = products;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
