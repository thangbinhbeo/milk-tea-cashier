using MilkTeaCashier.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int productId);
    }
}
