using Microsoft.Win32;
using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
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
		private string _selectedFilePath;
		private string _fileExtension;
		private readonly ImageUploadService _firebaseService;
		private readonly ProductService _productService;
		public Product EditProduct { get; set; }

		public ProductDetailView()
		{
			InitializeComponent();
			_productService = new ProductService();
			_firebaseService = new ImageUploadService();
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			// Retrieve values from the form fields
			string name = NameTextBox.Text;
			int category;
			string size = SizeTextBox.Text;
			double price;
			string url = "";

			/*// Validate the inputs
			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(size) || string.IsNullOrEmpty(PriceTextBox.Text) || string.IsNullOrEmpty(CategoryTextBox.Text))
			{
				MessageBox.Show("Please fill in all fields.");
				return;
			}*/

			if (!int.TryParse(CategoryTextBox.Text, out category))
			{
				MessageBox.Show("Invalid Category ID.");
				return;
			}

			if (!double.TryParse(PriceTextBox.Text, out price))
			{
				MessageBox.Show("Invalid Price.");
				return;
			}

			try
			{
				// If an image is selected, upload it
				if (!string.IsNullOrEmpty(_selectedFilePath))
				{
					string fileName = System.IO.Path.GetFileName(_selectedFilePath);
					url = await _firebaseService.UploadImageAsync(_selectedFilePath, _fileExtension);
				}
				else if (EditProduct == null)
				{
					MessageBox.Show("Please select an image.");
					return;
				}
				else
				{
					// If editing, retain the existing image URL
					url = EditProduct.Image;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error uploading image: {ex.Message}");
				return;
			}

			// Create or update the product model with the data
			if (EditProduct == null)
			{
				// Add new product (use CreateProductModel)
				var newProduct = new CreateProductModel
				{
					Name = name,
					CategoryId = category,
					Image = url,
					Price = price,
					Size = size,
					Status = "Available" // If needed, you can add the status here
				};

				var result = await _productService.AddProductAsync(newProduct);  // Pass CreateProductModel to AddProductAsync

				if (result != null)
				{
					MessageBox.Show("Product added successfully.");
				}
				else
				{
					MessageBox.Show("Failed to add product. Please try again.");
				}
			}
			else
			{
				// Update the existing product
				EditProduct.Name = name;
				EditProduct.CategoryId = category;
				EditProduct.Image = url;
				EditProduct.Price = price;
				EditProduct.Size = size;

				try
				{
					// Call the update method with the updated product
					await _productService.UpdateProductAsync(EditProduct);

					MessageBox.Show("Product updated successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Failed to update product. Error: {ex.Message}");
				}
			}
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void SelectImageButton_Click(object sender, RoutedEventArgs e)
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
	}
}
