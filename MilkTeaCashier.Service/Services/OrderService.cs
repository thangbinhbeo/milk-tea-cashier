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
using System.Drawing;
using System.Drawing.Printing;
using MilkTeaCashier.Data.DTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

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
            await _unitOfWork.OrderRepository.SaveAsync();
        }


        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _unitOfWork.OrderRepository.FindByConditionAsync(o => o.CreatedAt.Value == date.Date);
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

        public async Task PrintBillToPrinter(int orderId)
        {
            try
            {
                var order = _unitOfWork.OrderRepository.GetOrderDetail();
                var orderPrint = order
                    .Where(c => c.OrderId == orderId)
                    .Select(o => new OrderPrintModel
                    {
                        OrderId = o.OrderId,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        IsStay = o.IsStay,
                        Note = o.Note,
                        NumberTableCard = o.NumberTableCard,
                        PaymentMethod = o.PaymentMethod,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        EmployeeName = o.Employee.FullName,
                        CustomerName = o.CustomerId.HasValue ? o.Customer.Name : o.CustomerName,
                        Score = o.CustomerId.HasValue ? o.Customer.Score : null,
                        ProductPrint = o.OrderDetails.Select(od => new ProductPrint
                        {
                            ProductId = od.ProductId,
                            ProductName = od.Product.Name,
                            Size = od.Size,
                            Price = od.Price,
                            Quantity = od.Quantity
                        }).ToList()
                    }).FirstOrDefault();

                if (orderPrint == null)
                {
                    Console.WriteLine("Order not found.");
                    return;
                }

                PrintDocument printDocument = new PrintDocument();
                printDocument.PrinterSettings.PrinterName = "Microsoft Print to PDF";
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 850, 1100);

                printDocument.PrintPage += (sender, e) =>
                {
                    Graphics graphics = e.Graphics;
                    Font font = new Font("Arial", 10);
                    Brush brush = Brushes.Black;

                    float x = 10;
                    float y = 10;
                    float lineHeight = font.GetHeight();

                    graphics.DrawString($"Order ID: {orderPrint.OrderId}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"Employee: {orderPrint.EmployeeName}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"Customer: {orderPrint.CustomerName ?? "N/A"}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"Score: {orderPrint.Score ?? 0}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"NumberTable: {orderPrint.NumberTableCard ?? null}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"Payment Method: {orderPrint.PaymentMethod}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"IsStay: {orderPrint.IsStay}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"Total Amount: {orderPrint.TotalAmount:C}", font, brush, x, y);
                    y += lineHeight;
                    graphics.DrawString($"Date: {orderPrint.CreatedAt:C}", font, brush, x, y);
                    y += lineHeight;

                    graphics.DrawString("---- Order Details ----", font, brush, x, y);
                    y += lineHeight;

                    foreach (var detail in orderPrint.ProductPrint)
                    {
                        graphics.DrawString($"{detail.ProductName} - Size: {detail.Size} - Quantity: {detail.Quantity} - Price: {detail.Price:C}", font, brush, x, y);
                        y += lineHeight;
                    }

                    graphics.DrawString($"Note: {orderPrint.Note ?? "No notes"}", font, brush, x, y);
                };

                printDocument.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error printing bill: " + ex.Message);
            }
        }
    }

}
