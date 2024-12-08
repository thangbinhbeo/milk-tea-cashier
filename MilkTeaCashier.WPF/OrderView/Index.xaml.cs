using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace MilkTeaCashier.WPF.OrderView
{
	/// <summary>
	/// Interaction logic for Index.xaml
	/// </summary>
	public partial class Index : Window
	{
		private readonly OrderService _orderService;
		public Index()
		{
			InitializeComponent();
			_orderService = new OrderService();
			LoadOrders();
		}

		private async void LoadOrders()
		{
			try
			{
				var orders = await _orderService.GetAllOrdersAsync();
				if (orders != null)
				{
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
			var createOrderWindow = new Create();
			createOrderWindow.Closed += (s, args) => LoadOrders();
			createOrderWindow.ShowDialog();
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private async void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedOrder = OrdersDataGrid.SelectedItem as OrderDto;
			if (selectedOrder == null)
			{
				MessageBox.Show("Please select an order to view details.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
			var order = await _orderService.GetOrderByIdAsync(selectedOrder.Id);

			var detailWindow = new Detail(order);
			detailWindow.Closed += (s, args) => LoadOrders();
			detailWindow.ShowDialog();
		}

		private async void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
		{
			var selectedOrder = OrdersDataGrid.SelectedItem as OrderDto;
			if (selectedOrder == null)
			{
				MessageBox.Show("Please select an order to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var confirmationResult = MessageBox.Show(
				$"Are you sure you want to delete the order with ID {selectedOrder.Id}?",
				"Confirm Deletion",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning
			);

			if (confirmationResult == MessageBoxResult.Yes)
			{
				try
				{
					var resultMessage = await _orderService.DeleteOrderByIdAsync(selectedOrder.Id);
					MessageBox.Show(resultMessage, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

					LoadOrders();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error deleting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}
