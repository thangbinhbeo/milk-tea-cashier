using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;

namespace MilkTeaCashier.Service.Services
{
    public class ReportingService : IReportingService
    {
        #region ___FIELDS AND PROPERTIES___
        private readonly UnitOfWork _unitOfWork;
        public ReportingService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region ___OVERRIDDEN METHODS___
        public async Task<double> GetDailySalesAsync(DateTime date)
        {
            var orders = await _unitOfWork.OrderRepository.FindByConditionAsync(
                o => o.CreatedAt.Value.Date == date.Date && o.Status == OrderStatus.Completed.ToString());

            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<double> GetMonthlySalesAsync(int year, int month)
        {
            var orders = await _unitOfWork.OrderRepository.FindByConditionAsync(
                o => o.CreatedAt.Value.Year == year &&
                     o.CreatedAt.Value.Month == month &&
                     o.Status == OrderStatus.Completed.ToString());

            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<double> GetAnnualSalesAsync(int year)
        {
            var orders = await _unitOfWork.OrderRepository.FindByConditionAsync(
                o => o.CreatedAt.Value.Year == year && 
                (o.Status == OrderStatus.Completed.ToString() || o.Status == OrderStatus.Pending.ToString()));

            return orders.Sum(o => o.TotalAmount);
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _unitOfWork.OrderRepository.FindByConditionAsync(o => o.CreatedAt.Value.Date == date.Date);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _unitOfWork.OrderRepository.FindByConditionAsync(o => o.Status == status.ToString());
        }

        public async Task<List<RevenueReportDto>> GetRevenueReportAsync(DateTime startDate, DateTime endDate)
        {
            var orderList = await _unitOfWork.OrderRepository.FindByConditionAsync(
                o => o.CreatedAt >= startDate && o.CreatedAt <= endDate && o.Status == OrderStatus.Completed.ToString());

            return orderList
                .GroupBy(o => o.CreatedAt.Value.Date)
                .Select(group => new RevenueReportDto
                {
                    ReportDate = group.Key,
                    TotalRevenue = group.Sum(o => o.TotalAmount),
                    CompletedOrders = group.Count(),
                    PaymentSummaries = group.GroupBy(o => o.PaymentMethod)
                        .Select(pm => new PaymentMethodSummary
                        {
                            PaymentMethod = pm.Key,
                            Revenue = pm.Sum(o => o.TotalAmount)
                        }).ToList()
                }).ToList();
        }

        public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate)
        {
            var orderDetailList = await _unitOfWork.OrderDetailRepository.FindByConditionAsync
            (
                od => od.Order.CreatedAt >= startDate && od.Order.CreatedAt <= endDate,
                include: query => query.Include(od => od.Product)
            );

            return orderDetailList
                .Where(od => od.Product != null) // Ensure Product is not null
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(group => new TopSellingProductDto
                {
                    ProductID = group.Key.ProductId,
                    ProductName = group.Key.Name,
                    QuantitySold = group.Sum(od => od.Quantity),
                    Revenue = group.Sum(od => od.Price * od.Quantity)
                })
                .ToList();

        }
        public List<RevenueReportDto> FilterRevenueReports(
            DateTime startDate,
            DateTime endDate,
            double? minRevenue = null,
            double? maxRevenue = null)
        {
            // Step 1: Fetch and filter orders
            var ordersQuery = _unitOfWork.OrderRepository.GetOrderDetail()
                .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date >= startDate && o.CreatedAt.Value.Date <= endDate && o.Status == OrderStatus.Completed.ToString());

            var orders = ordersQuery.ToList();

            // Step 2: Group by date and ensure OrderDetails are included
            var groupedOrders = orders
                .Where(o => o.OrderDetails != null && o.OrderDetails.Any())
                .GroupBy(o => o.CreatedAt.Value.Date);

            // Step 3: Build RevenueReportDto
            var revenueReports = groupedOrders.Select(group => new RevenueReportDto
            {
                ReportDate = group.Key,
                TotalRevenue = group.Sum(o => o.TotalAmount),
                CompletedOrders = group.Count(),
                PaymentSummaries = group
                    .Where(o => o.PaymentMethod != null)
                    .GroupBy(o => o.PaymentMethod)
                    .Select(paymentGroup => new PaymentMethodSummary
                    {
                        PaymentMethod = paymentGroup.Key,
                        Revenue = paymentGroup.Sum(o => o.TotalAmount)
                    }).ToList(),
                CategorySummaries = GetCategorySummaries(group.SelectMany(o => o.OrderDetails))
            }).ToList();

            if (minRevenue.HasValue)
            {
                revenueReports = revenueReports.Where(r => r.TotalRevenue >= minRevenue.Value).ToList();
            }

            if (maxRevenue.HasValue)
            {
                revenueReports = revenueReports.Where(r => r.TotalRevenue <= maxRevenue.Value).ToList();
            }

            return revenueReports;
        }

        private List<CategorySummary> GetCategorySummaries(IEnumerable<OrderDetail> orderDetails)
        {
            if (orderDetails == null)
                return new List<CategorySummary>();

            return orderDetails
                .Where(od => od.Product != null && od.Product.Category != null)
                .GroupBy(od => od.Product.Category.CategoryName ?? "Uncategorized")
                .Select(categoryGroup => new CategorySummary
                {
                    CategoryName = categoryGroup.Key,
                    Revenue = categoryGroup.Sum(od => od.Price * od.Quantity)
                })
                .ToList();
        }

       
        public List<TopSellingProductDto> FilterTopSellingProducts(
            List<TopSellingProductDto> topSellingProducts,
            int topN
        )
        {
            return topSellingProducts
                .OrderByDescending(p => p.QuantitySold)
                .Take(topN)
                .ToList();
        }

        public string PrepareExportData(IEnumerable<RevenueReportDto> revenueReports, IEnumerable<TopSellingProductDto> topSellingProducts)
        {
            var exportedData = new StringBuilder();

            AppendRevenueReports(revenueReports, exportedData);
            AppendTopSellingProducts(topSellingProducts, exportedData);

            return exportedData.ToString();
        }
        #endregion

        #region ___HELPER METHODS___
        public List<string> GetAvailableCategories()
        {
            // Fetch all categories and handle null or empty cases
            return _unitOfWork.CategoryRepository.FindAll()
                .Where(c => !string.IsNullOrWhiteSpace(c.CategoryName))
                .Select(c => c.CategoryName)
                .Distinct()
                .ToList();
        }

        public List<string> GetAvailablePaymentMethods()
        {
            // Fetch all payment methods and handle null or empty cases
            return _unitOfWork.OrderRepository.FindAll()
                .Where(o => !string.IsNullOrWhiteSpace(o.PaymentMethod))
                .Select(o => o.PaymentMethod)
                .Distinct()
                .ToList();
        }

        private void AppendRevenueReports(IEnumerable<RevenueReportDto> revenueReports, StringBuilder exportedData)
        {
            exportedData.AppendLine("Revenue Reports:");
            if (revenueReports?.Any() == true)
            {
                foreach (var report in revenueReports)
                {
                    exportedData.AppendLine($"Date: {report.ReportDate:yyyy-MM-dd}, Total Revenue: {report.TotalRevenue:C}, Completed Orders: {report.CompletedOrders}");
                    AppendPaymentSummaries(report.PaymentSummaries, exportedData);
                }
            }
            else
            {
                exportedData.AppendLine("\tNo revenue data available.");
            }
        }

        private void AppendPaymentSummaries(IEnumerable<PaymentMethodSummary> paymentSummaries, StringBuilder exportedData)
        {
            if (paymentSummaries?.Any() == true)
            {
                foreach (var payment in paymentSummaries)
                {
                    exportedData.AppendLine($"\tPayment Method: {payment.PaymentMethod}, Revenue: {payment.Revenue:C}");
                }
            }
        }

        private void AppendTopSellingProducts(IEnumerable<TopSellingProductDto> topSellingProducts, StringBuilder exportedData)
        {
            exportedData.AppendLine("\nTop Selling Products:");
            if (topSellingProducts?.Any() == true)
            {
                foreach (var product in topSellingProducts)
                {
                    exportedData.AppendLine($"ProductID: {product.ProductID}, Product Name: {product.ProductName}, Total Sold: {product.QuantitySold}, Revenue: {product.Revenue:C}");
                }
            }
            else
            {
                exportedData.AppendLine("\tNo top-selling products data available.");
            }
        }
        #endregion
    }

}
