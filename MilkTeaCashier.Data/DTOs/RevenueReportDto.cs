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
        public DateTime ReportDate { get; set; }
        public double TotalRevenue { get; set; }
        public int CompletedOrders { get; set; }
        public List<PaymentMethodSummary> PaymentSummaries { get; set; }
    }
    public class PaymentMethodSummary
    {
        public string PaymentMethod { get; set; }
        public double Revenue { get; set; }
    }
}
