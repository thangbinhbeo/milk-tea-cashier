using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
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

namespace MilkTeaCashier.WPF.OrderView
{
	/// <summary>
	/// Interaction logic for Detail.xaml
	/// </summary>
	public partial class Detail : Window
	{
		private readonly OrderService _orderService;
		private Order _order;
		private ObservableCollection<OrderDetailDto> _selectedProducts;
		
		public Detail(Order order)
		{
			InitializeComponent();
			_orderService = new OrderService();
			_order = order;
            SubtotalTextBlock.Text = _order.TotalAmount.ToString();
            LoadData();
			_selectedProducts = new ObservableCollection<OrderDetailDto>();  
			ProductsDataGrid.ItemsSource = _selectedProducts;
        }

		private async void LoadData()
		{
			try
			{
				var productList = await _orderService.GetAllProductsAsync();
				if (productList == null || !productList.Any())
				{
					MessageBox.Show("No products found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				OrderForm.DataContext = _order;
				var productsList = await _orderService.GetAllOrderDetailByOrderId(_order.OrderId);
				foreach (var product in productsList)
				{
					if (!_selectedProducts.Any(p => p.ProductId == product.ProductId)) 
					{
						_selectedProducts.Add(product);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void SaveOrderButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
                var billPrint = new BillPreviewWindow(_order.OrderId);
				billPrint.ShowDialog();
            }
			catch (Exception ex)
			{
				//MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				string errorMessage = $"An error occurred: {ex.Message}";

				// Log inner exception details
				if (ex.InnerException != null)
				{
					errorMessage += $"\nInner Exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}";
				}

				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
