using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;
using System.Drawing;
using System.Drawing.Printing;
using MilkTeaCashier.Data.DTOs;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace MilkTeaCashier.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly UnitOfWork _unitOfWork;

        public OrderService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<OrderResponse> PlaceOrderAsync(CreateNewOrderDto model)
        {
            if (model == null || model.orderDetails == null || !model.orderDetails.Any())
                throw new ArgumentException("Order or OrderDetails cannot be null or empty.");

            var a = await _unitOfWork.TableCardRepository.FindByConditionAsync(a => a.NumberTableCard == model.NumberTableCard);
            if (a.Any())
            {
                return new OrderResponse
                {
                    OrderId = 0,
                    Message = "Please choose other TableNumber",
                };
            }

            Order order = new Order
            {
                CustomerName = model.CustomerName,
                CustomerId = model.CustomerId,
                NumberTableCard = model.NumberTableCard,
                IsStay = model.IsStay,
                Note = model.Note,
                PaymentMethod = model.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Status = OrderStatus.Pending.ToString(),
                EmployeeId = 7
            };

            // Calculate total
            order.TotalAmount = model.orderDetails.Sum(d => d.Quantity * d.Price);

            if (model.CustomerId != null)
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(model.CustomerId.Value);
                if (customer != null)
                {
                    customer.Score ??= 0;

                    customer.Score += (long)Math.Round(order.TotalAmount / 10);
                }
                await _unitOfWork.CustomerRepository.UpdateAsync(customer);
            }

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.OrderRepository.SaveAsync();
            int orderID = order.OrderId;

            if (order.NumberTableCard != null)
            {
                await _unitOfWork.TableCardRepository.AddAsync(new TableCard
                {
                    NumberTableCard = (int)order.NumberTableCard
                });
                await _unitOfWork.TableCardRepository.SaveAsync();
            }

            foreach (var detail in model.orderDetails)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = detail.ProductId,
                    Size = detail.Size,
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    Status = detail.OrderDetailStatus,
                };
                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            }

            await _unitOfWork.OrderDetailRepository.SaveAsync();

            return new OrderResponse
            {
                OrderId = orderID,
                Message = "Place Order Successfully!",
            };
        }

        public async Task<string> UpdateOrderAsync(int orderId, CreateNewOrderDto model)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                order.CustomerName = model.CustomerName;
                order.IsStay = model.IsStay;

                //update NumberTableCard
                if (model.NumberTableCard != order.NumberTableCard)
                {
                    if (model.IsStay == false && model.NumberTableCard == null)
                    {
                        var tableCard = await _unitOfWork.TableCardRepository.FindByConditionAsync(a => a.NumberTableCard == order.NumberTableCard);
                        foreach (var item in tableCard)
                        {
                            await _unitOfWork.TableCardRepository.RemoveAsync(item);
                        }
                        order.NumberTableCard = null;
                    }
                    else
                    {
                        var a = await _unitOfWork.TableCardRepository.FindByConditionAsync(a => a.NumberTableCard == model.NumberTableCard);
                        if (a.Any())
                        {
                            return "Please choose other TableNumber";
                        }
                        order.NumberTableCard = model.NumberTableCard;
                        await _unitOfWork.TableCardRepository.AddAsync(new TableCard
                        {
                            NumberTableCard = (int)order.NumberTableCard
                        });
                        await _unitOfWork.TableCardRepository.SaveAsync();
                    }
                }

                if (order.IsStay == true && order.NumberTableCard == null)
                {
                    return "Please choose one TableNumberCard";
                }

                if (model.Status == "Canceled")
                {
                    var orderDetails = await _unitOfWork.OrderDetailRepository.FindByConditionAsync(o => o.OrderId == orderId);
                    foreach (var orderDetail in orderDetails)
                    {
                        orderDetail.Status = "Canceled";
                        await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                    }
                }
                else
                {
                    // Lấy danh sách OrderDetail hiện tại trong db
                    var existingOrderDetails = await _unitOfWork.OrderDetailRepository.FindByConditionAsync(o => o.OrderId == orderId);

                    // Lấy danh sách OrderDetail từ model
                    var newOrderDetails = model.orderDetails ?? new List<OrderDetailDto>();

                    // Xóa các OrderDetail không còn tồn tại trong danh sách mới
                    foreach (var existingDetail in existingOrderDetails)
                    {
                        if (!newOrderDetails.Any(o => o.ProductId == existingDetail.ProductId))
                        {
                            await _unitOfWork.OrderDetailRepository.RemoveAsync(existingDetail);
                        }
                    }

                    // Thêm hoặc cập nhật OrderDetail
                    foreach (var newDetail in newOrderDetails)
                    {
                        var existingDetail = existingOrderDetails.FirstOrDefault(o => o.ProductId == newDetail.ProductId);
                        if (existingDetail != null)
                        {
                            // Cập nhật OrderDetail nếu đã tồn tại
                            existingDetail.Quantity = newDetail.Quantity;
                            existingDetail.Price = newDetail.Price;
                            existingDetail.Status = newDetail.OrderDetailStatus;
                            await _unitOfWork.OrderDetailRepository.UpdateAsync(existingDetail);
                        }
                        else
                        {
                            // Thêm OrderDetail mới nếu chưa tồn tại
                            var orderDetail = new OrderDetail
                            {
                                OrderId = orderId,
                                ProductId = newDetail.ProductId,
                                Quantity = newDetail.Quantity,
                                Price = newDetail.Price,
                                Status = newDetail.OrderDetailStatus,
                            };
                            await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
                        }
                    }
                }

                order.TotalAmount = model.orderDetails.Sum(d => d.Quantity * d.Price);
                order.Status = model.Status;
                order.PaymentMethod = model.PaymentMethod;
                order.Note = model.Note;
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.OrderDetailRepository.SaveAsync();
                await _unitOfWork.OrderRepository.SaveAsync();

                return "Update Order Successfully!";
            }
            catch (Exception ex)
            {
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<string> DeleteOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

                if (order == null)
                {
                    return "Order not found!";
                }

                var orderDetials = await _unitOfWork.OrderDetailRepository.FindByConditionAsync(o => o.OrderId == orderId);
                foreach (var orderDetail in orderDetials)
                {
                    await _unitOfWork.OrderDetailRepository.RemoveAsync(orderDetail);
                }

                var numbertable = await _unitOfWork.TableCardRepository.FindByConditionAsync(a => a.NumberTableCard == order.NumberTableCard);
                foreach (var card in numbertable)
                {
                    await _unitOfWork.TableCardRepository.RemoveAsync(card);
                }

                var result = await _unitOfWork.OrderRepository.RemoveAsync(order);
                if (result)
                {
                    return "Delete Order Successfully!";
                }
                else
                {
                    return "Failed to delete Order.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<List<OrderDto>> GetAllOrdersAsync()
        {
            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllAsync();
                if (orders == null || orders.Count == 0)
                {
                    return null;
                }

                var orderList = orders
                    .OrderByDescending(order => order.CreatedAt)
                    .Select(order => new OrderDto
                    {
                        Id = order.OrderId,
                        CustomerName = order.CustomerName,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status,
                        PaymentMethod = order.PaymentMethod,
                        NumberTableCard = order.NumberTableCard,
                        CreatedAt = order.CreatedAt,
                    }).ToList();

                return orderList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _unitOfWork.ProductRepository.GetAllAsync();
        }

        public async Task<List<OrderDetailDto>> GetAllOrderDetailByOrderId(int orderId)
        {
            var orderDetails = await _unitOfWork.OrderDetailRepository.FindByConditionAsync(a => a.OrderId == orderId);
            List<OrderDetailDto> result = new List<OrderDetailDto>();
            foreach (var orderDetail in orderDetails)
            {
                result.Add(new OrderDetailDto
                {
                    ProductId = orderDetail.ProductId,
                    Price = orderDetail.Price,
                    Quantity = orderDetail.Quantity,
                    Size = orderDetail.Size,
                    OrderDetailStatus = orderDetail.Status,
                    ProductName = (await _unitOfWork.ProductRepository.GetByIdAsync(orderDetail.ProductId)).Name,
                });
            }
            return result;
        }

        public async Task<List<OrderDto>> SearchOrdersAsync(DateTime? date, string? customerName, string? status, int? orderId)
        {
            var orders = await GetAllOrdersAsync();

            if (date.HasValue)
                orders = orders.Where(o => o.CreatedAt.Value.Date == date.Value.Date).ToList();

            if (!string.IsNullOrEmpty(customerName))
                orders = orders.Where(o => o.CustomerName.Contains(customerName, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(status) && status != "All")
                orders = orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

            if (orderId.HasValue)
                orders = orders.Where(o => o.Id == orderId).ToList();

            return orders;
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

        public async Task<OrderPrintModel> GetByIdOrder(int orderId)
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
                return null;
            } else
            {
                return orderPrint;
            }
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
                    graphics.DrawString($"Date: {orderPrint.CreatedAt}", font, brush, x, y);
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

        public void ExportBillToPdf(int orderId, string filePath)
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

                PdfDocument pdf = new PdfDocument();
                pdf.Info.Title = $"Bill_{orderPrint.OrderId}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf";

                PdfPage page = pdf.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Arial", 14, XFontStyleEx.Bold);
                XFont headerFont = new XFont("Arial", 12, XFontStyleEx.Regular);
                XFont regularFont = new XFont("Arial", 10, XFontStyleEx.Regular);
                XBrush brush = XBrushes.Black;

                double margin = 40;
                double x = margin, y = margin;

                gfx.DrawString($"Bill for Order {orderPrint.OrderId}", titleFont, brush, x, y);
                y += 30;


                gfx.DrawString($"Employee: {orderPrint.EmployeeName}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Customer: {orderPrint.CustomerName ?? "N/A"}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Score: {orderPrint.Score ?? 0}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Payment Method: {orderPrint.PaymentMethod}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Number Table: {orderPrint.NumberTableCard ?? 0}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Total Amount: {orderPrint.TotalAmount:C}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Is Stay: {orderPrint.IsStay}", headerFont, brush, x, y);
                y += 20;
                gfx.DrawString($"Order Date: {orderPrint.CreatedAt:yyyy-MM-dd HH:mm:ss}", headerFont, brush, x, y);
                y += 40;

                gfx.DrawString("---- Order Details ----", titleFont, brush, x, y);
                y += 20;

                double tableStartY = y;
                double tableHeight = 20;
                double tableWidth = page.Width - 2 * margin; 
                double columnWidth1 = 150;
                double columnWidth2 = 80; 
                double columnWidth3 = 80; 
                double columnWidth4 = 100; 

                gfx.DrawString("Product Name", headerFont, brush, x, tableStartY);
                gfx.DrawString("Size", headerFont, brush, x + columnWidth1, tableStartY);
                gfx.DrawString("Quantity", headerFont, brush, x + columnWidth1 + columnWidth2, tableStartY);
                gfx.DrawString("Price", headerFont, brush, x + columnWidth1 + columnWidth2 + columnWidth3, tableStartY);
                y += tableHeight;

                gfx.DrawLine(XPens.Black, x, y, tableWidth + margin, y);
                y += tableHeight;

                foreach (var detail in orderPrint.ProductPrint)
                {
                    gfx.DrawString(detail.ProductName, regularFont, brush, x, y);
                    gfx.DrawString(detail.Size, regularFont, brush, x + columnWidth1, y);
                    gfx.DrawString(detail.Quantity.ToString(), regularFont, brush, x + columnWidth1 + columnWidth2, y);
                    gfx.DrawString($"{detail.Price:C}", regularFont, brush, x + columnWidth1 + columnWidth2 + columnWidth3, y);
                    y += tableHeight;

                    gfx.DrawLine(XPens.Black, x, y, tableWidth + margin, y);
                }

                y += 20;
                gfx.DrawString($"Note: {orderPrint.Note ?? "No notes"}", regularFont, brush, x, y);

                pdf.Save(filePath);

                Console.WriteLine($"Bill has been successfully saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error exporting bill to PDF: " + ex.Message);
            }

        }
    }

}
