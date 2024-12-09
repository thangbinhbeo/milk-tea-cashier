using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using System;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IOrderService
    {
        Task<string> PlaceOrderAsync(CreateNewOrderDto model);
		Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
        Task<Order> GetOrderByIdAsync(int orderId);
        double CalculateTotalAmount(List<OrderDetail> orderDetails);
        Task<List<Product>> GetAllProductsAsync();

	}
}
