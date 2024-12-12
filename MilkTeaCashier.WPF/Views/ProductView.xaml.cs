using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.Repository;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            LoadProducts();
        }

		private void AddProduct_Click(object sender, RoutedEventArgs e)
		{
			var productDetailView = new ProductDetailView();
			var dialogResult = productDetailView.ShowDialog();
			LoadProducts();
		}

		private void EditProduct_Click(object sender, RoutedEventArgs e)
		{
			var selectedProduct = ProductsDataGrid.SelectedItem as Product;
			if (selectedProduct != null)
			{
				this.Close();

				var productDetailView = new ProductDetailView
				{
					EditProduct = selectedProduct 
				};
				bool? dialogResult = productDetailView.ShowDialog();
				LoadProducts();
			}
			else
			{
				MessageBox.Show("Please select a product to edit.", "Edit Product", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
		{
			var selectedProduct = ProductsDataGrid.SelectedItem as Product;

			if (selectedProduct == null)
			{
				MessageBox.Show("Please select a product to delete.");
				return;
			}

			var result = MessageBox.Show($"Are you sure you want to delete the product '{selectedProduct.Name}'?",
										  "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				try
				{

					bool isDeleted = await _service.DeleteProductAsync(selectedProduct.ProductId);

					if (isDeleted)
					{
						MessageBox.Show("Product deleted successfully.");
						var productsList = ProductsDataGrid.ItemsSource as ObservableCollection<Product>;
						productsList?.Remove(selectedProduct);
						LoadProducts();
					}
					else
					{
						MessageBox.Show("Failed to delete the product. Please try again.");
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"An error occurred while deleting the product: {ex.Message}");
				}
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
				LoadProducts();
			}
		}

		private async void LoadProducts()
		{
			try
			{
				ProductsDataGrid.IsEnabled = false;

				var products = await _service.GetAllProductsAsync();

				if (products != null && products.Any())
				{
					ProductsDataGrid.ItemsSource = products;
				}
				else
				{
					MessageBox.Show("No products available.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				ProductsDataGrid.IsEnabled = true;
			}
		}

        private void ManageCategory_Click(object sender, RoutedEventArgs e)
		{
			var categoryManagement = new CategoryManagement();
			categoryManagement.ShowDialog();
		}

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
