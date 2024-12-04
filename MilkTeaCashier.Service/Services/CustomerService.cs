using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly UnitOfWork _unitOfWork;

        // Constructor accepting UnitOfWork
        public CustomerService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task CreateCustomerAsync(CreateCustomerDto customerDto, int currentEmployeeId)
        {
            if (customerDto == null) throw new ArgumentNullException(nameof(customerDto));

            var newCustomer = new Customer
            {
                Name = customerDto.Name,
                Phone = customerDto.Phone,
                Gender = customerDto.Gender,
                Score = customerDto.Score,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = GetCurrentEmployeeName(currentEmployeeId),
                UpdatedAt = null,
                UpdatedBy = null
            };

            await _unitOfWork.CustomerRepository.AddAsync(newCustomer);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

       

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _unitOfWork.CustomerRepository.GetAllAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }
            return customer; // Return the customer
        }

        public async Task UpdateCustomerAsync(int customerId, UpdateCustomerDto customerDto, int currentEmployeeId)
        {
            if (customerDto == null) throw new ArgumentNullException(nameof(customerDto));

            var existingCustomer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            // Update properties
            existingCustomer.Name = customerDto.Name;
            existingCustomer.Phone = customerDto.Phone;
            existingCustomer.Gender = customerDto.Gender;
            existingCustomer.Score = customerDto.Score;
            existingCustomer.UpdatedAt = DateTime.UtcNow;
            existingCustomer.UpdatedBy = GetCurrentEmployeeName(currentEmployeeId);

            _unitOfWork.CustomerRepository.Update(existingCustomer);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            _unitOfWork.CustomerRepository.Remove(customer);
            await _unitOfWork.SaveChangesWithTransactionAsync();
        }

        public string GetCurrentEmployeeName(int employeeId)
        {
            var employee = _unitOfWork.EmployeeRepository.GetById(employeeId); // Use Unit of Work to get employee
            return employee?.FullName; // Return the full name or null if not found
        }
    }
}