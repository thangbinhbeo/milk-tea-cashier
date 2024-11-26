﻿using MilkTeaCashier.Data.Base;
using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.Repository
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public CategoryRepository() { }
        public CategoryRepository(PRN212_MilkTeaCashierContext context)
        {
            _context = context;
        }
    }
}
