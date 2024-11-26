using MilkTeaCashier.Data.Models;
using MilkTeaCashier.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.UnitOfWork
{
    public class UnitOfWork
    {
        public PRN212_MilkTeaCashierContext _unitOfWorkContext;

        private CategoryRepository _categoryRepository;
        private CustomerRepository _customerRepository;
        private EmployeeRepository _employeeRepository;
        private OrderDetailRepository _orderDetailRepository;
        private OrderRepository _orderRepository;
        private ProductRepository _productRepository;
        private TableCardRepository _tableCardRepository;

        public UnitOfWork()
        {
            _unitOfWorkContext ??= new PRN212_MilkTeaCashierContext();
        }

        public UnitOfWork(PRN212_MilkTeaCashierContext unitOfWorkContext)
        {
            _unitOfWorkContext ??= unitOfWorkContext;
        }

        public CategoryRepository CategoryRepository
        {
            get
            {
                return _categoryRepository ??= new CategoryRepository(_unitOfWorkContext);
            }
        }
        
        public CustomerRepository CustomerRepository
        {
            get
            {
                return _customerRepository ??= new CustomerRepository(_unitOfWorkContext);
            }
        }
        
        public EmployeeRepository EmployeeRepository
        {
            get
            {
                return _employeeRepository ??= new EmployeeRepository(_unitOfWorkContext);
            }
        }
        
        public OrderDetailRepository OrderDetailRepository
        {
            get
            {
                return _orderDetailRepository ??= new OrderDetailRepository(_unitOfWorkContext);
            }
        }
        
        public OrderRepository OrderRepository
        {
            get
            {
                return _orderRepository ??= new OrderRepository(_unitOfWorkContext);
            }
        }
        
        public ProductRepository ProductRepository
        {
            get
            {
                return _productRepository ??= new ProductRepository(_unitOfWorkContext);
            }
        }
        
        public TableCardRepository TableCardRepository
        {
            get
            {
                return _tableCardRepository ??= new TableCardRepository(_unitOfWorkContext);
            }
        }

        ////TO-DO CODE HERE/////////////////

        #region Set transaction isolation levels

        /*
        Read Uncommitted: The lowest level of isolation, allows transactions to read uncommitted data from other transactions. This can lead to dirty reads and other issues.

        Read Committed: Transactions can only read data that has been committed by other transactions. This level avoids dirty reads but can still experience other isolation problems.

        Repeatable Read: Transactions can only read data that was committed before their execution, and all reads are repeatable. This prevents dirty reads and non-repeatable reads, but may still experience phantom reads.

        Serializable: The highest level of isolation, ensuring that transactions are completely isolated from one another. This can lead to increased lock contention, potentially hurting performance.

        Snapshot: This isolation level uses row versioning to avoid locks, providing consistency without impeding concurrency. 
         */

        public int SaveChangesWithTransaction()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _unitOfWorkContext.Database.BeginTransaction())
            {
                try
                {
                    result = _unitOfWorkContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }

        public async Task<int> SaveChangesWithTransactionAsync()
        {
            int result = -1;

            //System.Data.IsolationLevel.Snapshot
            using (var dbContextTransaction = _unitOfWorkContext.Database.BeginTransaction())
            {
                try
                {
                    result = await _unitOfWorkContext.SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    //Log Exception Handling message                      
                    result = -1;
                    dbContextTransaction.Rollback();
                }
            }

            return result;
        }
        #endregion

    }
}
