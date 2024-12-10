using MilkTeaCashier.Data.DTOs;
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
    public class CategoryService : ICategoryService
    {
        private readonly UnitOfWork _unitOfWork;

        public CategoryService()
        {
            _unitOfWork ??= new UnitOfWork();
        }

        public async Task<List<Category>> GetAllCategory()
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                if (categories == null || categories.Count == 0)
                {
                    return null;
                }

                return categories;
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<List<Category>> SearchCategories(string? name, string? description, DateTime? createdAt)
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

                if (categories == null || categories.Count == 0)
                {
                    return null; 
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    name = name.ToLower();
                    categories = categories.Where(c => c.CategoryName != null && c.CategoryName.ToLower().Contains(name)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    description = description.ToLower();
                    categories = categories.Where(c => c.Description != null && c.Description.ToLower().Contains(description)).ToList();
                }

                if (createdAt.HasValue)
                {
                    categories = categories.Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Date <= createdAt.Value.Date).ToList();
                }

                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error occurred while searching for categories: ", ex);
            }
        }


        public async Task<Category> GetById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if(category == null)
                {
                    return null;
                }

                return category;
            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<string> CreateNewCategory(CreateNewCategory category)
        {
            try
            {
                var newCategory = new Category
                {
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                
                var result = await _unitOfWork.CategoryRepository.CreateAsync(newCategory);
                if (result > 0)
                {
                    return "Create Category Successfully!";
                } 
                else
                {
                    return "Failed to create category.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<string> DeleteById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return "Category not found!";
                }

                var result = await _unitOfWork.CategoryRepository.RemoveAsync(category);
                if (result)
                {
                    return "Delete Category Successfully!";
                }
                else
                {
                    return "Failed to delete category.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error: ", ex);
            }
        }

        public async Task<string> UpdateCategory(int id, CreateNewCategory category)
        {
            try
            {
                var cate = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (cate == null)
                {
                    return "Category not found!";
                }

                cate.UpdatedAt = DateTime.Now;
                cate.CategoryName = category.CategoryName;
                cate.Description = category.Description;

                var result = await _unitOfWork.CategoryRepository.UpdateAsync(cate);
                if (result > 0)
                {
                    return "Update Category Successfully!";
                }
                else
                {
                    return "Failed to update category.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error: ", ex);
            }
        }
    }
}
