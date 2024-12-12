using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.DTOs.OrderDTO
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double SubTotal { get; set; }
    }
}
