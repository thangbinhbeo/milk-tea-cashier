using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IReportingService
    {
        Task<double> GetDailySalesAsync(DateTime date);
        Task<double> GetMonthlySalesAsync(int year, int month);
        Task<double> GetAnnualSalesAsync(int year);
        Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);

        Task<List<RevenueReportDto>> GetRevenueReportAsync(DateTime startDate, DateTime endDate);
        Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate);

        List<RevenueReportDto> FilterRevenueReports
        (
            DateTime startDate,
            DateTime endDate,
            double? minRevenue = null,
            double? maxRevenue = null
        );


        List<TopSellingProductDto> FilterTopSellingProducts
        (
            List<TopSellingProductDto> topSellingProducts,
            int topN
        );

        List<string> GetAvailableCategories();
        List<string> GetAvailablePaymentMethods();
        string PrepareExportData
        (
            IEnumerable<RevenueReportDto> revenueReports,
            IEnumerable<TopSellingProductDto> topSellingProducts
        );
    }

}
