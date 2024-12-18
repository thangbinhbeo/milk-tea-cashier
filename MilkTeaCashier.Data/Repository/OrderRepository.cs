﻿using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.Repository
{
    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository() { }
        public OrderRepository(PRN212_MilkTeaCashierContext context)
        {
            _context = context;
        }

        public IQueryable<Order> GetOrderDetail()
        {
            return _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Customer)
                .Include(o => o.Employee);
        }
	}
}
