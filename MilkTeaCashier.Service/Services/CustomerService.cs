using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.Repository;

namespace MilkTeaCashier.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly GenericRepository<Employee> _employeeRepository;
        private readonly GenericRepository<Customer> _customerRepository;
        private readonly PRN212_MilkTeaCashierContext _context;
        public CustomerService(GenericRepository<Customer> customerRepository, GenericRepository<Employee> employeeRepository)
        {
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _context = new PRN212_MilkTeaCashierContext();
        }

        

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllAsync(c => c.Orders);
        }

        public async Task<Customer> AddCustomerAsync(CreateCustomerDto customerDto)
        {
            if (customerDto == null) throw new ArgumentNullException(nameof(customerDto));

            if (await CustomerExistsAsync(customerDto.Phone))
            {
                throw new InvalidOperationException("A customer with the same phone number already exists.");
            }

            var newCustomer = new Customer
            {
                Name = customerDto.Name,
                Phone = customerDto.Phone,
                Gender = customerDto.Gender,
                //Score = customerDto.Score,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = null,
                UpdatedAt = null,
                UpdatedBy = null
            };

            await _customerRepository.AddAsync(newCustomer);
            await _customerRepository.SaveAsync();

            return newCustomer;
        }

        public async Task UpdateCustomerAsync(int customerId, string name = null, string phone = null, string gender = null)
        {
            // Validate input
            if (customerId <= 0)
                throw new ArgumentException("Customer ID must be a positive integer.", nameof(customerId));

            // Find the existing customer in the database
            var existingCustomer = await _context.Customers.FindAsync(customerId);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }


            if (!string.IsNullOrEmpty(phone) && phone != existingCustomer.Phone)
            {
                // Check if the new phone already exists for another customer
                if (await CustomerExistsAsync(phone, customerId)) // Pass null for name
                {
                    throw new InvalidOperationException("Another customer with the same phone number already exists.");
                }
            }

            // Update only the fields that are not null
            if (!string.IsNullOrEmpty(name))
            {
                existingCustomer.Name = name;
            }

            if (!string.IsNullOrEmpty(phone))
            {
                existingCustomer.Phone = phone;
            }

            if (!string.IsNullOrEmpty(gender))
            {
                existingCustomer.Gender = gender;
            }

            existingCustomer.UpdatedAt = DateTime.UtcNow;
            existingCustomer.UpdatedBy = null; 

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while saving the updated customer.", ex);
            }
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            _customerRepository.Remove(customer); 
            await _customerRepository.SaveAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomerByNameAndPhoneAsync(string name, string phone)
        {
            List<Customer> result = await _context.Customers.Include("Orders").ToListAsync();
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(phone))
            {
                return result;
            }
            // Filter by both name and phone
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(phone))
            {
                return result.Where(c => c.Name.ToLower().Contains(name.ToLower()) &&
                                         c.Phone.Contains(phone));
            }
            // Filter by name if phone is not provided
            if (!string.IsNullOrEmpty(name))
            {
                return result.Where(c => c.Name.ToLower().Contains(name.ToLower()));
            }
            // Filter by phone if name is not provided
            if (!string.IsNullOrEmpty(phone))
            {
                return result.Where(c => c.Phone.Contains(phone));
            }
            return result; 
        }


        public async Task<bool> CustomerExistsAsync(string phone, int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.Phone == phone);
        }
        public async Task<bool> CustomerExistsAsync(string phone, int? customerId = null)
        {
            var query = _context.Customers.AsQueryable();

            if (customerId.HasValue)
            {
                query = query.Where(c => c.CustomerId != customerId.Value); // Exclude the current customer
            }

            return await query.AnyAsync(c =>
                (phone != null && c.Phone == phone));
        }
    }
}