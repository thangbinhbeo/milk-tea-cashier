using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.UnitOfWork;
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
        private readonly UnitOfWork _unitOfWork;

        public EmployeeService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<Employee> AuthenticateAsync(string username, string password)
        {
            var employees = await _unitOfWork.EmployeeRepository.FindByConditionAsync(e => e.Username == username && e.PasswordHash == password);
            var employee = employees.FirstOrDefault();

            if (employee == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            return employee;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _unitOfWork.EmployeeRepository.GetAllAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            await _unitOfWork.EmployeeRepository.AddAsync(employee);
            await _unitOfWork.EmployeeRepository.SaveAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _unitOfWork.EmployeeRepository.PrepareUpdate(employee);
            await _unitOfWork.EmployeeRepository.SaveAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("Employee not found.");

            _unitOfWork.EmployeeRepository.PrepareRemove(employee);
            await _unitOfWork.EmployeeRepository.SaveAsync();
        }
    }

}
