using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface ICustomerService
    {
        Task CreateCustomerAsync(CreateCustomerDto customerDto, int currentEmployeeId);
        Task DeleteCustomerAsync(int customerId);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int customerId); 
        Task UpdateCustomerAsync(int customerId, UpdateCustomerDto customerDto, int currentEmployeeId); 
    }
}
