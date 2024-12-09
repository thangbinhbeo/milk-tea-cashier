using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.DTOs
{
    public class CreateProductModel
    {
		public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        public string Size { get; set; }

        public double Price { get; set; }

        public string Image { get; set; }

    }
}
