using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
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
		private ObservableCollection<CartItem> _selectedProducts;
		private List<CartItem> _listCart;
		private int _customerId = 0;
		private double _originalPrice = 0;

		private bool isLoyal = false;

		private int _employeeID;
		public Create(int employeeID)
		{
			InitializeComponent();
			_orderService ??= new OrderService();
			_customerService ??= new CustomerService();
			_selectedProducts = new ObservableCollection<CartItem>();  
			_employeeID = employeeID;
			ProductsDataGrid.ItemsSource = _selectedProducts; 
		}
		
		public Create(int employeeID, List<CartItem> cartList)
		{
			InitializeComponent();
			_orderService ??= new OrderService();
			_customerService ??= new CustomerService();
			_selectedProducts = new ObservableCollection<CartItem>();  
			_employeeID = employeeID;
			ProductsDataGrid.ItemsSource = _selectedProducts; 
			_listCart = cartList;

			ProductsDataGrid.ItemsSource = _listCart;
			SubtotalTextBlock.Text = _listCart.Sum(o => o.SubTotal).ToString();
        }

		private async void AddProductButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var cart = new ProductCart(_employeeID);
				cart.ShowDialog();

				this.Close();
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
				string orderStatus = "Completed";
				string paymentMethod = (PaymentMethodComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
				string note = NoteTextBox.Text.Trim();

				// Kiểm tra xem các thông tin quan trọng đã được nhập chưa
				if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(tableNumber) || string.IsNullOrEmpty(orderStatus))
				{
					MessageBox.Show("Please fill in all required fields (Customer Name, Table Number, and Order Status).", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (ProductsDataGrid.ItemsSource == null)
				{
					MessageBox.Show("There is none of products to paid !", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

				newOrder.orderDetails = _listCart.Select(p => new OrderDetailDto
				{
					ProductId = p.ProductId,
					ProductName = p.Name,
					Size = p.Size,
					Price = p.Price,
					Quantity = p.Quantity,
					OrderDetailStatus = "Pending",
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
				_customerId = customerId;

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

        private async void PointUse(object sender, RoutedEventArgs e)
		{
            try
            {
				if (int.Parse(SubtotalTextBlock.Text.Trim()) == 0)
				{
                    MessageBox.Show("There aren't any products !", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
				} 
				else
				{
                    var customer = await _customerService.GetCustomerByIdAsync(_customerId);

                    int price = int.Parse(SubtotalTextBlock.Text.Trim());
					_originalPrice = price;
					int score = customer.Score != null ? (int)customer.Score : 0;
                    if (score >= price)
                    {
						SubtotalTextBlock.Text = (0).ToString();
                    }
                    else
                    {
                        SubtotalTextBlock.Text = (price - score).ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error discount point: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
		
		private void PointNoUse(object sender, RoutedEventArgs e)
		{
            try
            {
                SubtotalTextBlock.Text = _originalPrice.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error discount point: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
