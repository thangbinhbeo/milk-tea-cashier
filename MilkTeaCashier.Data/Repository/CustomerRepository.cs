using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.Repository
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        public CustomerRepository() { }
        public CustomerRepository(PRN212_MilkTeaCashierContext context)
        {
            _context = context;
        }
        //public async Task<IEnumerable<Customer>> GetCustomersAsync()
        //{
        //    var ListCustomer = new List<Customer>();
        //    using var dbContext = new PRN212_MilkTeaCashierContext();
        //    ListCustomer = await dbContext.Customers.Include(c => c.Orders).ToListAsync();
        //    return ListCustomer;
        //}
    }
}
