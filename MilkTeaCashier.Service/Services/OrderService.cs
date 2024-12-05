using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;

namespace MilkTeaCashier.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;

        public OrderService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task PlaceOrderAsync(Order order, List<OrderDetail> orderDetails)
        {
            if (order == null || orderDetails == null || !orderDetails.Any())
                throw new ArgumentException("Order or OrderDetails cannot be null or empty.");

            // Ensure proper defaults
            order.CreatedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            order.Status = OrderStatus.Pending.ToString();

            // Calculate total
            order.TotalAmount = orderDetails.Sum(d => d.Quantity * d.Price);

            // Add order and order details
            await _unitOfWork.OrderRepository.AddAsync(order);
            foreach (var detail in orderDetails)
            {
                detail.OrderId = order.OrderId;
                await _unitOfWork.OrderDetailRepository.AddAsync(detail);
            }

            // Save changes
            await _unitOfWork.OrderRepository.SaveAsync();
        }


        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _unitOfWork.OrderRepository.FindByConditionAsync(o => o.CreatedAt.Value == date.Date);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
        }

        public double CalculateTotalAmount(List<OrderDetail> orderDetails)
        {
            return orderDetails.Sum(detail => detail.Quantity * detail.Price);
        }

        public async Task PrintBillToPrinter(int orderId)
        {
            try
            {


            }
            catch (Exception ex)
            {

            }
        }
    }

}
