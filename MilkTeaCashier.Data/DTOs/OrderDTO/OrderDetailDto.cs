using MilkTeaCashier.Data.Models;

namespace MilkTeaCashier.Data.DTOs.OrderDTO
{
	public class OrderDetailDto
	{
		public int ProductId { get; set; }

		public string ProductName { get; set; }

		public string Size { get; set; }

		public int Quantity { get; set; }

		public double Price { get; set; }

		public string OrderDetailStatus { get; set; }
	}
}
