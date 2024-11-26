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
    }

}
