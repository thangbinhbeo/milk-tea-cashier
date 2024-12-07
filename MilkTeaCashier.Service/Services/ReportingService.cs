using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                o => o.CreatedAt.Value.Year == year && o.Status == OrderStatus.Completed.ToString());

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
            var orderDetailList = await _unitOfWork.OrderDetailRepository.FindByConditionAsync(
                od => od.Order.CreatedAt >= startDate && od.Order.CreatedAt <= endDate);

            return orderDetailList
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(group => new TopSellingProductDto
                {
                    ProductID = group.Key.ProductId,
                    ProductName = group.Key.Name,
                    QuantitySold = group.Sum(od => od.Quantity),
                    Revenue = group.Sum(od => od.Price * od.Quantity)
                }).ToList();
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
