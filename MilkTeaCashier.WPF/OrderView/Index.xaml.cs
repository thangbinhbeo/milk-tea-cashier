using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace MilkTeaCashier.WPF.OrderView
{
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window
	{
		private readonly OrderService _orderService;
		private int _employeeID;
		public Index(int employeeID)
		{
			InitializeComponent();
			_orderService = new OrderService();
			_employeeID = employeeID;
			LoadOrders();
		}

		public async void LoadOrders()
		{
			try
			{
				var orders = await _orderService.GetAllOrdersAsync();
				if (orders != null)
				{
					OrdersDataGrid.ItemsSource = null;
					OrdersDataGrid.ItemsSource = orders;
				}
				else
				{
					MessageBox.Show("No orders found!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddOrderButton_Click(object sender, RoutedEventArgs e)
		{
			var createOrderWindow = new Create(_employeeID);
			createOrderWindow.Closed += (s, args) => LoadOrders();
			createOrderWindow.ShowDialog();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private async void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedOrder = OrdersDataGrid.SelectedItem as Order;
			if (selectedOrder == null)
			{
				MessageBox.Show("Please select an order to view details.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			var order = await _orderService.GetOrderByIdAsync(selectedOrder.OrderId);

			var detailWindow = new Detail(order);
			detailWindow.Closed += (s, args) => LoadOrders();
			detailWindow.ShowDialog();
		}

		private async void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedOrder = OrdersDataGrid.SelectedItem as Order;
			if (selectedOrder == null)
			{
				MessageBox.Show("Please select an order to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var confirmationResult = MessageBox.Show(
				$"Are you sure you want to delete the order with ID {selectedOrder.OrderId}?",
				"Confirm Deletion",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning
			);

			if (confirmationResult == MessageBoxResult.Yes)
			{
				try
				{
					var resultMessage = await _orderService.DeleteOrderByIdAsync(selectedOrder.OrderId);
					MessageBox.Show(resultMessage, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

					LoadOrders();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private async void SearchButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var searchDate = SearchDatePicker.SelectedDate;
				var searchCustomer = SearchCustomerTextBox.Text;
				var searchStatus = (SearchStatusComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
				var searchOrderIdText = SearchOrderIdTextBox.Text;

				int.TryParse(searchOrderIdText, out int searchOrderId);

				var orders = await _orderService.SearchOrdersAsync(searchDate,searchCustomer,searchStatus,searchOrderId);

				OrdersDataGrid.ItemsSource = orders;

				if (!orders.Any())
				{
					MessageBox.Show("No orders found with the given criteria.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error during search: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
