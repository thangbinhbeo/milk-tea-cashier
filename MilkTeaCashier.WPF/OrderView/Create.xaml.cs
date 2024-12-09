﻿using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.Views;
using Org.BouncyCastle.Security.Certificates;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace MilkTeaCashier.WPF.OrderView
{
	/// <summary>
	/// Interaction logic for Create.xaml
	/// </summary>
	public partial class Create : Window
	{
		private readonly OrderService _orderService;
		private readonly CustomerService _customerService;
		private ObservableCollection<OrderDetailDto> _selectedProducts;

		private bool isLoyal = false;

		private int _employeeID;
		public Create(int employeeID)
		{
			InitializeComponent();
			_orderService ??= new OrderService();
			_customerService ??= new CustomerService();
			_selectedProducts = new ObservableCollection<OrderDetailDto>();  
			_employeeID = employeeID;
			ProductsDataGrid.ItemsSource = _selectedProducts; 
			LoadProduct();
		}

		private async void LoadProduct()
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

		private async void SaveOrderButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
                string customerName;
                int? customerId = null;

				if (RegisterPointCheckBox.IsChecked == true)
				{
                    if (CustomerComboBox.SelectedItem is ComboBoxItem selectedCustomer)
                    {
                        customerName = selectedCustomer.Content.ToString();
                        customerId = (int?)selectedCustomer.Tag;
                    }
                    else
                    {
                        MessageBox.Show("Please select a customer from the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
				else
				{
                    customerName = CustomerNameTextBox.Text.Trim();
                }

                // 1. Thu thập thông tin từ các trường trong giao diện
				string tableNumber = TableNumberTextBox.Text.Trim();
				bool isStay = IsStayCheckBox.IsChecked.GetValueOrDefault();
				string orderStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
				string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
				string note = NoteTextBox.Text.Trim();

				// Kiểm tra xem các thông tin quan trọng đã được nhập chưa
				if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(tableNumber) || string.IsNullOrEmpty(orderStatus))
				{
					MessageBox.Show("Please fill in all required fields (Customer Name, Table Number, and Order Status).", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (!_selectedProducts.Any())
				{
					MessageBox.Show("Please choose at least 1 Product.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				int customerLoyal = 0;
				if (isLoyal)
				{
                    string pointsText = CustomerPointsTextBlock.Text;

                    string pointsValue = pointsText.Replace("Total Points: ", "").Trim();
					customerLoyal = int.Parse(pointsValue);
                }

				// 3. Tạo đối tượng đơn hàng
				var newOrder = new CreateNewOrderDto
				{
					CustomerName = customerName,
					CustomerId = customerId,
					NumberTableCard = int.Parse(tableNumber),
					IsStay = isStay,
					Status = orderStatus,
					Note = note,
					PaymentMethod = paymentMethod,
					EmployeeId = _employeeID,
					CustomerScore = customerLoyal
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


				// 4. Lưu đơn hàng (gọi dịch vụ lưu đơn hàng)
				var result = await _orderService.PlaceOrderAsync(newOrder);
				var billPrint = new BillPreviewWindow(result.OrderId);

				MessageBox.Show(result.Message.ToString());
				this.Close();
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

		private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				// Lấy sản phẩm được chọn trong DataGrid
				var selectedProduct = ProductsDataGrid.SelectedItem as OrderDetailDto;

				if (selectedProduct == null)
				{
					MessageBox.Show("Please select a product to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				// Loại bỏ sản phẩm khỏi danh sách
				_selectedProducts.Remove(selectedProduct);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while removing the product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

        private async void RegisterPointCheckBox_Checked(object sender, RoutedEventArgs e)
        {
			try
			{
				isLoyal = true;
                CustomerComboBox.Visibility = Visibility.Visible;
                CustomerNameTextBox.Visibility = Visibility.Collapsed;

                CustomerPointsTextBlock.Visibility = Visibility.Visible;
                ApplyPointsCheckBox.Visibility = Visibility.Visible;

                var customers = await _customerService.GetAllCustomers();

                if (customers != null && customers.Any())
                {
                    CustomerComboBox.Items.Clear();

                    foreach (var customer in customers)
                    {
                        CustomerComboBox.Items.Add(new ComboBoxItem
                        {
                            Content = customer.Name,
                            Tag = customer.CustomerId
                        });
                    }
                }
                else
                {
                    MessageBox.Show("No customers found.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
			catch(Exception ex)
			{
                MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                var customerId = (int)selectedItem.Tag;

                var customer = await _customerService.GetCustomerByIdAsync(customerId);

                if (customer != null)
                {
                    CustomerPointsTextBlock.Text = $"Total Points: {customer.Score}";
                }
                else
                {
                    MessageBox.Show("Customer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RegisterPointCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CustomerComboBox.Visibility = Visibility.Collapsed;
            CustomerNameTextBox.Visibility = Visibility.Visible;
            CustomerPointsTextBlock.Visibility = Visibility.Collapsed;
            ApplyPointsCheckBox.Visibility = Visibility.Collapsed;
        }

        private void CreateCustomerButton_Click(object sender, RoutedEventArgs e)
		{
			var createCustomerWindow = new CreateCustomerWindow(_employeeID);
			createCustomerWindow.ShowDialog();
		}

        private void ProductsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Price" || e.Column.Header.ToString() == "Quantity")
            {
                UpdateSubtotal();
            }
        }

        private void UpdateSubtotal()
        {
            double subtotal = 0;

            foreach (var item in ProductsDataGrid.Items)
            {
                if (item is OrderDetailDto product)
                {
                    double price = product.Price;
                    int quantity = product.Quantity;

                    subtotal += price * quantity;
                }
            }

            SubtotalTextBlock.Text = subtotal.ToString("C2");
        }
    }
}
