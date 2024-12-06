using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.DTOs
{
    public class OrderPrintModel
    {
        //Order ne
        public int OrderId { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
        public bool? IsStay { get; set; }
        public string Note { get; set; }
        public int? NumberTableCard { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        //NhanVien ne
        public string EmployeeName { get; set; }
        //KhachHang ne
        public string CustomerName { get; set; }
        public long? Score { get; set; }
        //SanPham ne
        public List<ProductPrint> ProductPrint { get; set; }

    }

    public class ProductPrint
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
