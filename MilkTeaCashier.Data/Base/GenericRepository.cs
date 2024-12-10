using Microsoft.EntityFrameworkCore;
using MilkTeaCashier.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Data.Base
{
    public class GenericRepository<T> where T : class
    {
        protected PRN212_MilkTeaCashierContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository()
        {
            _context ??= new PRN212_MilkTeaCashierContext();
            _dbSet = _context.Set<T>();
        }

        public GenericRepository(PRN212_MilkTeaCashierContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        #region Separating assign entity and save operators

        public void PrepareCreate(T entity)
        {
            _context.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity); // Add entity asynchronously
        }

        public void PrepareUpdate(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }

        public void PrepareRemove(T entity)
        {
            _context.Remove(entity);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion Separating assign entity and save operators

        public IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).AsNoTracking();
        }

        public virtual async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<T>> FindByConditionAsync
        (
            Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>> include = null
        )
        {
            IQueryable<T> query = _context.Set<T>().Where(expression).AsNoTracking();

            // Apply the include function if provided
            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync();
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
		public async Task<List<Order>> GetAllOrdersAsync()
		{
			return await _context.Orders
								 .AsNoTracking()
								 .Include(o => o.OrderDetails)
								 .ToListAsync();
		}

		public List<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.ToList();
        }

        public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        public void Create(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        public async Task<int> CreateAsync(T entity)
        {
            await _context.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;

            return await _context.SaveChangesAsync();
        }

        public bool Remove(T entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public T GetById(string code)
        {
            return _context.Set<T>().Find(code);
        }

        public async Task<T> GetByIdAsync(string code)
        {
            return await _context.Set<T>().FindAsync(code);
        }
        #region ___HELPER METHODS___

        /// <summary>
        /// Retrieves all entities from the database.
        /// </summary>
        /// <returns>All entities as an IQueryable for further filtering.</returns>
        public IQueryable<T> FindAll()
        {
            return _dbSet.AsNoTracking();
        }

        /// <summary>
        /// Retrieves all entities matching a specified condition.
        /// </summary>
        /// <param name="expression">The condition to filter entities.</param>
        /// <returns>Filtered entities as an IQueryable.</returns>
        public IQueryable<T> FindByConditionAsQueryable(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).AsNoTracking();
        }

        /// <summary>
        /// Selects specific properties or transformations from the entity set.
        /// </summary>
        /// <typeparam name="TResult">The type of the transformed result.</typeparam>
        /// <param name="selector">The selector function to transform entities.</param>
        /// <returns>Selected entities as an IEnumerable.</returns>
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return _dbSet.AsNoTracking().Select(selector).ToList();
        }

        /// <summary>
        /// Retrieves distinct entities based on a specified property or selector.
        /// </summary>
        /// <typeparam name="TKey">The type of the property to use for distinctness.</typeparam>
        /// <param name="keySelector">The selector function to determine distinctness.</param>
        /// <returns>Distinct entities as an IEnumerable.</returns>
        public IEnumerable<T> Distinct<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            return _dbSet.AsNoTracking().GroupBy(keySelector).Select(g => g.First()).ToList();
        }

        #endregion New Methods
    }
}

