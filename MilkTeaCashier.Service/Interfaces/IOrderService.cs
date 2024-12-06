using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using System;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IOrderService
    {
        Task PlaceOrderAsync(CreateNewOrderDto order);
        Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
        Task<OrderDto> GetOrderByIdAsync(int orderId);
        double CalculateTotalAmount(List<OrderDetail> orderDetails);
    }
}
