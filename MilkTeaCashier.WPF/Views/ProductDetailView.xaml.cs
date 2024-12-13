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
		private readonly CategoryService _categoryService;

		private bool isEdit = false;
		public Product EditProduct { get; set; }

		public ProductDetailView()
		{
            InitializeComponent();
			_productService = new ProductService();
			_firebaseService = new ImageUploadService(); 
			_categoryService = new CategoryService();
            LoadCatgories();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			string name = NameTextBox.Text;
			int category;
			string size = SizeTextBox.SelectedItem != null ? SizeTextBox.SelectedItem.ToString() : null;
			double price;
			string url = "";
			string status = StatusTextBox.SelectedItem != null ? StatusTextBox.SelectedItem.ToString() : null;

            // Validate the inputs
            if (string.IsNullOrEmpty(name) || SizeTextBox.SelectedItem == null || string.IsNullOrEmpty(PriceTextBox.Text) || CategoryComboBox.SelectedItem == null)
			{
				MessageBox.Show("Please fill in all fields.");
				return;
			}

			if (!double.TryParse(PriceTextBox.Text, out price))
			{
				MessageBox.Show("Invalid Price.");
				return;
			}

			if (price < 0)
			{
				MessageBox.Show("Price cannot be less than 0.");
				return;
			}

			try
			{
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
					url = EditProduct.Image;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error uploading image: {ex.Message}");
				return;
			}

			var selected = CategoryComboBox.SelectedItem as Category;
			category = selected.CategoryId;

            if (EditProduct == null)
			{
				var newProduct = new CreateProductModel
				{
					Name = name,
					CategoryId = category,
					Image = url,
					Price = price,
					Size = size,
					Status = "Available"
				};

				var result = await _productService.AddProductAsync(newProduct);  

				if (result != null)
				{
					MessageBox.Show("Product added successfully.");

                    this.DialogResult = true;
                    this.Close();
                }
				else
				{
					MessageBox.Show("Failed to add product. Please try again.");
				}
			}
			else
			{
				EditProduct.Name = name;
				EditProduct.CategoryId = 8;
				EditProduct.Image = url;
				EditProduct.Price = price;
				EditProduct.Size = size;
				EditProduct.Status = status;

				var updatePro = new CreateProductModel
				{
					CategoryId = category,
					Image = url,
					Price = price,
					Size = size,
					Status = status,
					Name = name
				};

				try
				{
					await _productService.UpdateProductAsync(EditProduct.ProductId, updatePro);

					MessageBox.Show("Product updated successfully.");
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Failed to update product. Error: {ex.Message}");
				}
				this.DialogResult = true;
				var view = new ProductView();
				view.ShowDialog();

				this.Close();
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

		protected override void OnContentRendered(EventArgs e)
		{
			base.OnContentRendered(e);

			if (EditProduct != null)
			{
				var category = _categoryService.GetById(EditProduct.CategoryId);

                ProductIdTextBox.Text = EditProduct.ProductId.ToString();
				NameTextBox.Text = EditProduct.Name;
				CategoryComboBox.Text = category != null ? category.Result.CategoryName : "";
				SizeTextBox.Text = EditProduct.Size;
				PriceTextBox.Text = EditProduct.Price.ToString();
				StatusTextBox.Text = EditProduct.Status.ToString();

				// Load the image if present
				if (!string.IsNullOrEmpty(EditProduct.Image))
				{
					ProductImage.Source = new BitmapImage(new Uri(EditProduct.Image));
				}
			}
		}

		public async void LoadCatgories()
		{
			var categories = await _categoryService.GetAllCategory();
			CategoryComboBox.ItemsSource = categories;

            var currentItems = StatusTextBox.ItemsSource as List<string>;
			if (currentItems != null)
			{
				currentItems.Add("Available");
				currentItems.Add("Unavailable");

				StatusTextBox.ItemsSource = null;
				StatusTextBox.ItemsSource = currentItems;
			}
			else
			{
				StatusTextBox.ItemsSource = new List<string> { "Available", "Unavailable" };
			}

			var items = SizeTextBox.ItemsSource as List<string>;
			if (items != null)
			{
				items.Add("M");
				items.Add("S");
				items.Add("L");
				items.Add("XL");

				SizeTextBox.ItemsSource = null;
				SizeTextBox.ItemsSource = items;
			}
			else
			{
				SizeTextBox.ItemsSource = new List<string> { "M", "S", "L", "XL" };
			}

			if (EditProduct != null)
			{
				var category = EditProduct.CategoryId;
				var status = EditProduct.Status;
				var size = EditProduct.Size;
				ProductIdTextBox.IsEnabled = false;

                foreach (var item in StatusTextBox.ItemsSource)
				{
					if (item.Equals(status))
					{
						StatusTextBox.SelectedItem = status;
					}
				}

				foreach (var item in SizeTextBox.ItemsSource)
				{
					if (item.Equals(size))
					{
						SizeTextBox.SelectedItem = size;
					}
				}

				foreach (var item in CategoryComboBox.ItemsSource)
				{
					if (item.Equals(category))
					{
						CategoryComboBox.SelectedItem = category;
					}
				}

				var cate = await _categoryService.GetById(category);

                if (cate != null)
                {
					CategoryComboBox.SelectedValue = cate.CategoryId;
                }
            } 
			else
			{
				IdLabel.Visibility = Visibility.Collapsed;
				ProductIdTextBox.Visibility = Visibility.Collapsed;
				StatusLabel.Visibility = Visibility.Collapsed;
				StatusTextBox.Visibility = Visibility.Collapsed;
            }

		}
	}
}
