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

				ProductComboBox.ItemsSource = productList.Select(p => p.Name).ToList();
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

		private async void AddProductButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Kiểm tra xem người dùng đã chọn sản phẩm chưa
				var selectedProductName = ProductComboBox.SelectedItem?.ToString();
				if (string.IsNullOrEmpty(selectedProductName))
				{
					MessageBox.Show("Please select a product.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Tải danh sách sản phẩm bất đồng bộ
				var productList = await _orderService.GetAllProductsAsync();
				var product = productList.FirstOrDefault(p => p.Name == selectedProductName);

				if (product == null)
				{
					MessageBox.Show("Selected product not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Tạo một đối tượng Product trong DataGrid
				var productToAdd = new OrderDetailDto
				{
					ProductId = product.ProductId,
					ProductName = product.Name,
					Size = product.Size, 
					Price = product.Price, 
					Quantity = 1, 
					OrderDetailStatus = "Pending",
				};

				// Thêm vào danh sách
				_selectedProducts.Add(productToAdd);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while adding the product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var selectedProduct = ProductsDataGrid.SelectedItem as OrderDetailDto;

				if (selectedProduct == null)
				{
					MessageBox.Show("Please select a product to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				_selectedProducts.Remove(selectedProduct);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while removing the product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async void SaveOrderButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// thông tin từ các trường trong giao diện
				int OrderId = int.Parse(OrderIdTextBox.Text.Trim()); 
				string customerName = CustomerNameTextBox.Text.Trim();
				string tableNumber = TableNumberTextBox.Text.Trim();
				int? tableNumberValue = string.IsNullOrEmpty(tableNumber) ? (int?)null : int.Parse(tableNumber);
				bool isStay = IsStayCheckBox.IsChecked.GetValueOrDefault();
				string orderStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
				string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
				string note = NoteTextBox.Text.Trim();

				if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(paymentMethod) || string.IsNullOrEmpty(orderStatus))
				{
					MessageBox.Show("Please fill in all required fields (Customer Name, Payment Method, and Order Status).", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (!_selectedProducts.Any())
				{
					MessageBox.Show("Please choose at least 1 Product.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Tạo đơn hàng
				var newOrder = new CreateNewOrderDto
				{
					CustomerName = customerName,
					NumberTableCard = tableNumberValue,
					IsStay = isStay,
					Status = orderStatus,
					Note = note,
					PaymentMethod = paymentMethod,
				};

				newOrder.orderDetails = _selectedProducts.Select(p => new OrderDetailDto
				{
					ProductId = p.ProductId,
					ProductName = p.ProductName,
					Size = p.Size,
					Price = p.Price,
					Quantity = p.Quantity,
					OrderDetailStatus = p.OrderDetailStatus,
				}).ToList();


				// Lưu đơn hàng
				var result = await _orderService.UpdateOrderAsync(OrderId, newOrder);

				MessageBox.Show(result);

				this.Close();
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
