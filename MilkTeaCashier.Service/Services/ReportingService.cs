using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class ReportingService : IReportingService
    {
        private readonly GenericRepository<Order> _orderRepository;

        public ReportingService(GenericRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        // Get total sales for a specific day
        public async Task<double> GetDailySalesAsync(DateTime date)
        {
            var orders = await _orderRepository.FindByConditionAsync
            ( o => 
                o.CreatedAt.Value.Date == date.Date && 
                o.Status == OrderStatus.Completed
            );
            Console.WriteLine($"Today's sales: ${orders}");
            return orders.Sum(o => o.TotalAmount);
        }

        // Get total sales for a specific month
        public async Task<double> GetMonthlySalesAsync(int year, int month)
        {
            var orders = await _orderRepository.FindByConditionAsync
            (o =>
                o.CreatedAt.Value.Year == year &&
                o.CreatedAt.Value.Month == month &&
                o.Status == OrderStatus.Completed
            );
            Console.WriteLine($"This Month's sales: ${orders}");
            return orders.Sum(o => (double)o.TotalAmount);
        }
        // Get total sales for a specific yearl
        public async Task<double> GetAnnualSalesAsync(int year)
        {
            var orders = await _orderRepository.FindByConditionAsync
            (o =>
                o.CreatedAt.Value.Year == year &&
                o.Status == OrderStatus.Completed
            );
            Console.WriteLine($"This Year's sales: ${orders}");
            return orders.Sum(o => (double)o.TotalAmount);
        }

        // Get all orders for a specific date
        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _orderRepository.FindByConditionAsync(o => o.CreatedAt.Value.Date == date.Date);
        }

        // Get all orders by status (e.g., Pending, Completed, Canceled)
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _orderRepository.FindByConditionAsync(o => o.Status == status);
        }

    }


}
