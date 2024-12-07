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
			_service = new ProductService();
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (EditProduct == null)
			{
				var newProduct = new Product
				{
					ProductId = int.Parse(ProductIdTextBox.Text),
					CategoryId = int.Parse(CategoryComboBox.SelectedValue.ToString()),
					Name = NameTextBox.Text,
					Size = SizeTextBox.Text,
					Price = double.Parse(PriceTextBox.Text),
					Status = StatusTextBox.Text
				};
				await _service.AddProductAsync(newProduct);
			}
			else
			{
				EditProduct.CategoryId = int.Parse(CategoryComboBox.SelectedValue.ToString());
				EditProduct.Name = NameTextBox.Text;
				EditProduct.Size = SizeTextBox.Text;
				EditProduct.Price = double.Parse(PriceTextBox.Text);
				EditProduct.Status = StatusTextBox.Text;

				await _service.UpdateProductAsync(EditProduct);
			}

			this.Close();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (EditProduct != null)
			{
				FillElements(EditProduct);
			}
			FillComboBox();
		}

		private void FillElements(Product product)
		{
			if(product != null)
			{
				ProductIdTextBox.Text = product.ProductId.ToString();
				CategoryComboBox.SelectedValue = product.CategoryId;
				NameTextBox.Text = product.Name;
				SizeTextBox.Text = product.Size;
				PriceTextBox.Text = product.Price.ToString();
				StatusTextBox.Text = product.Status;
			}
		}

		private void FillComboBox()
		{
			CategoryComboBox.ItemsSource = _service.GetAllCategories();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
