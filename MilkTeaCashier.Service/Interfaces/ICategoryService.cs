using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategory();
        Task<Category> GetById(int id);
        Task<string> CreateNewCategory(CreateNewCategory category);
        Task<string> DeleteById(int id);
        Task<string> UpdateCategory(int id, CreateNewCategory category);
    }
}
