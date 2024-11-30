using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.DTOs
{
    public class TopSellingProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public double Revenue { get; set; }
    }
}
