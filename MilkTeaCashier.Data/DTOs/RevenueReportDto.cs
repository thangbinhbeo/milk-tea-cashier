using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.DTOs
{
    public class RevenueReportDto
    {
        public DateTime ReportDate { get; set; } // Date of the report
        public double TotalRevenue { get; set; } // Total revenue for the day
        public int CompletedOrders { get; set; } // Total completed orders
        public List<PaymentMethodSummary> PaymentSummaries { get; set; } // Revenue breakdown by payment method
        public List<CategorySummary> CategorySummaries { get; set; } // Revenue breakdown by category
    }

    public class PaymentMethodSummary
    {
        public string PaymentMethod { get; set; } // e.g., Cash, Credit Card
        public double Revenue { get; set; } // Revenue for this payment method
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; } // e.g., Drinks, Toppings
        public double Revenue { get; set; } // Revenue for this category
    }

}
