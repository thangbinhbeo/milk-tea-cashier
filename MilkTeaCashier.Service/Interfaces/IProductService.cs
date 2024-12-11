using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<string> AddProductAsync(CreateProductModel product);
        Task UpdateProductAsync(int id, CreateProductModel product);
        Task<bool> DeleteProductAsync(int productId);
    }
}
