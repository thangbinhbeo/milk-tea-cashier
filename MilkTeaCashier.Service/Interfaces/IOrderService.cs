using MilkTeaCashier.Data.Models;
using System;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IOrderService
    {
        Task PlaceOrderAsync(Order order, List<OrderDetail> orderDetails);
        Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
        Task<Order> GetOrderByIdAsync(int orderId);
        double CalculateTotalAmount(List<OrderDetail> orderDetails);
    }
}
