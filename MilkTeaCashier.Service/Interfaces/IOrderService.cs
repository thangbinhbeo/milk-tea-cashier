using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using System;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> PlaceOrderAsync(CreateNewOrderDto model);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<List<Product>> GetAllProductsAsync();
    }
}
