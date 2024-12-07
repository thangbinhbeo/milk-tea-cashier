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

        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer> AddCustomerAsync(CreateCustomerDto customerDto);
        Task UpdateCustomerAsync(int customerId, string name = null, string phone = null, string gender = null);

        Task DeleteCustomerAsync(int customerId);

        Task<IEnumerable<Customer>> SearchCustomerByNameAndPhoneAsync(string name, string phone);

    }
}
