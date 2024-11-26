using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly GenericRepository<Employee> _employeeRepository;

        public EmployeeService(GenericRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> AuthenticateAsync(string username, string password)
        {
            var employees = await _employeeRepository.FindByConditionAsync(e => e.Username == username && e.PasswordHash == password);
            var employee = employees.FirstOrDefault();

            if (employee == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employeeRepository.PrepareUpdate(employee);
            await _employeeRepository.SaveAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            _employeeRepository.PrepareRemove(employee);
            await _employeeRepository.SaveAsync();
        }
    }

}
