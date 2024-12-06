namespace MilkTeaCashier.Data.DTOs.OrderDTO
{
	public class OrderDto
	{
		public string CustomerName { get; set; }
		public int? NumberTableCard { get; set; }
		public bool? IsStay { get; set; }
		public string Note { get; set; }
		public string PaymentMethod { get; set; }
		public double TotalAmount { get; set; }
		public List<OrderDetailDto> orderDetails { get; set; }
	}
}
