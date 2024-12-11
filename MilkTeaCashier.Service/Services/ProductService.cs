using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.Base;
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
	public class ProductService : IProductService
	{
		private readonly UnitOfWork _unitOfWork;

		public ProductService()
		{
			_unitOfWork ??= new UnitOfWork();
		}

		public async Task<IEnumerable<Product>> GetAllProductsAsync()
		{
			return await _unitOfWork.ProductRepository.GetAll()  
				.Include(p => p.Category) 
				.ToListAsync();
		}

		public async Task<Product> GetProductByIdAsync(int productId)
		{
			return await _unitOfWork.ProductRepository.GetByIdAsync(productId);
		}

		public async Task<string> AddProductAsync(CreateProductModel product)
		{
			if (product == null)
				throw new ArgumentNullException(nameof(product));

			var newProduct = new Product
			{
				CategoryId = product.CategoryId,
				Name = product.Name,
				Size = product.Size,
				Price = product.Price,
				Image = product.Image,
				Status = "Available",
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};

			var result = await _unitOfWork.ProductRepository.CreateAsync(newProduct);
			if (result > 0)
			{
				return "Create Product Successfully!";
			}
			else
			{
				return "Create Fail!";
			}
		}

		public async Task UpdateProductAsync(int id, CreateProductModel product)
		{
			if (product == null)
				throw new ArgumentNullException(nameof(product));

			var update = new Product
			{
				CategoryId = product.CategoryId,
				Name = product.Name,
				Size = product.Size,
				Price = product.Price,
				Image = product.Image,
				ProductId = id,
				Status = product.Status,
				UpdatedAt = DateTime.Now,
			};

			_unitOfWork.ProductRepository.PrepareUpdate(update);
			await _unitOfWork.ProductRepository.SaveAsync();
		}

		public async Task<bool> DeleteProductAsync(int productId)
		{
			try
			{
				var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
				if (product == null)
				{
					return false;
				}

				_unitOfWork.ProductRepository.PrepareRemove(product);
				await _unitOfWork.ProductRepository.SaveAsync();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<IEnumerable<Product>> SearchProductsAsync(string searchText)
		{
			if (string.IsNullOrEmpty(searchText))
			{
				return await GetAllProductsAsync(); // Return all products if search text is empty
			}

			// Filter by product name only (case-insensitive search)
			var products = await _unitOfWork.ProductRepository.GetAllAsync();
			return products.Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
		}
	}
}
