﻿using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.Repository;
using System.Diagnostics;
using MilkTeaCashier.Data.UnitOfWork;

namespace MilkTeaCashier.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly PRN212_MilkTeaCashierContext _context;
        public CustomerService(GenericRepository<Customer> customerRepository, GenericRepository<Employee> employeeRepository)
        {
            _unitOfWork ??= new UnitOfWork();
            _context = new PRN212_MilkTeaCashierContext();
        }

        /// <summary>
        ///  Employee employee = await _employeeService.AuthenticateAsync(username, password);
       //  MessageBox.Show($"Welcome, {employee.FullName}!");
        // Store the logged-in employee in the session // phải thêm dòng ở dưới vào cái login để nó có tên nhan viên x.x
         //  CustomerService.SessionService.SetCurrentEmployee(employee); // Correctly reference the nested class
         /// </summary>
         /// <returns></returns>
       
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.Include(C => C.Orders).ToListAsync(); 
            Debug.WriteLine($"Fetched {customers.Count()} customers.");
            return customers;
        }

        public async Task<Customer> AddCustomerAsync(CreateCustomerDto customerDto)
        {
            if (customerDto == null) throw new ArgumentNullException(nameof(customerDto));

            if (await CustomerExistsAsync(customerDto.Phone))
            {
                throw new InvalidOperationException("A customer with the same phone number already exists.");
            }
            var currentEmployee = CustomerService.SessionService.GetCurrentEmployee();
            var newCustomer = new Customer
            {
                Name = customerDto.Name,
                Phone = customerDto.Phone,
                Gender = customerDto.Gender,
                Score = 0,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentEmployee?.FullName ?? "System",
                UpdatedAt = null,
                UpdatedBy = null
            };

            await _unitOfWork.CustomerRepository.AddAsync(newCustomer);
            await _unitOfWork.CustomerRepository.SaveAsync();

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


            var currentEmployee = SessionService.GetCurrentEmployee(); // Get the current employee

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

            // Update timestamps and user info
            existingCustomer.UpdatedAt = DateTime.UtcNow;
            existingCustomer.UpdatedBy = currentEmployee?.FullName ?? "System"; // Use current employee's name

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
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            _unitOfWork.CustomerRepository.Remove(customer);
            await _unitOfWork.CustomerRepository.SaveAsync();
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
        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
        }

        public static class SessionService
        {
            private static Employee _currentEmployee;

            public static void SetCurrentEmployee(Employee employee)
            {
                _currentEmployee = employee;
            }

            public static Employee GetCurrentEmployee()
            {
                return _currentEmployee;
            }
        }
    }
}