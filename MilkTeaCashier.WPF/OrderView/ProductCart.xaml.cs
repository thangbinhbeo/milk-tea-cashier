using MilkTeaCashier.Data.DTOs.OrderDTO;
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

namespace MilkTeaCashier.WPF.OrderView
{
    /// <summary>
    /// Interaction logic for ProductCart.xaml
    /// </summary>
    public partial class ProductCart : Window
    {
        private readonly ProductService _service;
        private int _employeeID;
        public ProductCart(int employeeID)
        {
            InitializeComponent();
            _service = new ProductService();
            LoadProducts();
            _employeeID = employeeID;
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

        private void AddToCartBtn(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Please select a product to add to the cart.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedProduct = ProductsDataGrid.SelectedItem as Product;

            if (SizeComboBox.SelectedItem == null || string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                MessageBox.Show("Please select a size and enter quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedSize = (SizeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            double adjustedPrice = selectedProduct.Price;

            switch (selectedSize)
            {
                case "M":
                    adjustedPrice += 5000;
                    break;
                case "XL":
                    adjustedPrice += 10000;
                    break;
                case "XXL":
                    adjustedPrice += 15000;
                    break;
            }

            var cartItem = new CartItem
            {
                ProductId = selectedProduct.ProductId,
                Name = selectedProduct.Name,
                Size = selectedSize,
                Quantity = quantity,
                Price = adjustedPrice,
                SubTotal = adjustedPrice * quantity
            };

            var cartItems = CartDataGrid.ItemsSource as List<CartItem> ?? new List<CartItem>();
            cartItems.Add(cartItem);
            CartDataGrid.ItemsSource = null;
            CartDataGrid.ItemsSource = cartItems;

            TotalTextBlock.Text = $"{cartItems.Sum(item => item.SubTotal):C}";

            SizeComboBox.SelectedItem = null;
            QuantityTextBox.Text = string.Empty;
        }

        private void RemoveItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (CartDataGrid.SelectedItem is CartItem selectedItem)
            {
                var cartItems = CartDataGrid.ItemsSource as List<CartItem>;
                if (cartItems != null)
                {
                    cartItems.Remove(selectedItem);
                    CartDataGrid.ItemsSource = null; 
                    CartDataGrid.ItemsSource = cartItems;

                    UpdateTotal(cartItems);
                }
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CartDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var cartItems = CartDataGrid.ItemsSource as List<CartItem>;
            if (cartItems == null) return;

            if (e.Row.Item is CartItem editedItem)
            {
                if (e.Column.Header.ToString() == "Size")
                {
                    var comboBox = e.EditingElement as ComboBox;
                    var selectedSize = comboBox?.SelectedItem as ComboBoxItem;
                    string newSize = selectedSize?.Content.ToString();

                    if (newSize != null)
                    {
                        editedItem.Size = newSize;
                        editedItem.Price = AdjustPriceBasedOnSize(editedItem.Price, newSize);
                        editedItem.SubTotal = editedItem.Price * editedItem.Quantity;
                    }
                }
                else if (e.Column.Header.ToString() == "Quantity")
                {
                    var textBox = e.EditingElement as TextBox;
                    if (int.TryParse(textBox?.Text, out int newQuantity) && newQuantity > 0)
                    {
                        editedItem.Quantity = newQuantity;
                        editedItem.SubTotal = editedItem.Price * editedItem.Quantity;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid quantity.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                CartDataGrid.ItemsSource = null;
                CartDataGrid.ItemsSource = cartItems;
                UpdateTotal(cartItems);
            }
        }

        private double AdjustPriceBasedOnSize(double basePrice, string size)
        {
            return size switch
            {
                "M" => basePrice + 5000,
                "XL" => basePrice + 10000,
                "XXL" => basePrice + 15000,
                _ => basePrice,
            };
        }

        private void UpdateTotal(List<CartItem> cartItems)
        {
            var total = cartItems.Sum(item => item.SubTotal);
            TotalTextBlock.Text = $"{total:C}";
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            var order = new Create(_employeeID);
            order.ShowDialog();

            this.Close();
        }

        private void SaveCartBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                var cartItems = CartDataGrid.ItemsSource as List<CartItem>;

                if (cartItems == null || !cartItems.Any())
                {
                    MessageBox.Show("There are no items in the cart to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var createWindow = new Create(_employeeID, cartItems);
                createWindow.ShowDialog();

                this.Close();
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error while saving cart: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
