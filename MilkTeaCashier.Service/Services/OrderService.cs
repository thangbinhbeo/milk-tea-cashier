using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly GenericRepository<Order> _orderRepository;
        private readonly GenericRepository<OrderDetail> _orderDetailRepository;
		private readonly UnitOfWork _unitOfWork;

		public OrderService(GenericRepository<Order> orderRepository, GenericRepository<OrderDetail> orderDetailRepository, UnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task PlaceOrderAsync(CreateNewOrderDto model)
        {
            if (model == null || model.orderDetails == null || !model.orderDetails.Any())
                throw new ArgumentException("Order or OrderDetails cannot be null or empty.");

            Order order = new Order
            {
                CustomerName = model.CustomerName,
                NumberTableCard = model.NumberTableCard,
                IsStay = model.IsStay,
                Note = model.Note,
                PaymentMethod = model.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending.ToString()
            };
            // Calculate total
            order.TotalAmount = model.orderDetails.Sum(d => d.Quantity * d.Price);

            // Add order and order details
            await _unitOfWork.OrderRepository.AddAsync(order);

            foreach (var detail in model.orderDetails)
            {
                OrderDetail orderDetail = new OrderDetail 
                {
					OrderId = order.OrderId,
				    ProductId = detail.ProductId,
                    Size = detail.Size,
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    Status = "Pending",
			    };
                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            }

            // Save changes
            await _orderRepository.SaveAsync();
        }


        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _orderRepository.FindByConditionAsync(o => o.CreatedAt.Value == date.Date);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int orderId)
        {
            Order order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
			OrderDto model = new OrderDto
			{
                CustomerName = order.CustomerName,
                NumberTableCard = order.NumberTableCard,
                IsStay = order.IsStay,
                Note = order.Note,
                PaymentMethod = order.PaymentMethod,
            };

            var orderDetails = await _unitOfWork.OrderDetailRepository.GetAllAsync();
            foreach(var od in orderDetails)
            {
                if (od.OrderId == orderId)
                {
                    model.orderDetails.Add(new OrderDetailDto
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product.Name,
                        Size = od.Size,
                        Price = od.Price,
                        Quantity = od.Quantity,
                    });
                }
            }

            return model;
        }

        public double CalculateTotalAmount(List<OrderDetail> orderDetails)
        {
            return orderDetails.Sum(detail => detail.Quantity * detail.Price);
        }
    }

}
