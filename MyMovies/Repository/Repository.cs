using Microsoft.EntityFrameworkCore;
using MyMovies.Data;
using MyMovies.Repository.IRepository;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MyMovies.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbset = _db.Set<T>();
        }
        public  IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var invludeprop in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries ))
                {
                    query = query.Include(invludeprop);
                }
            }
            return query.ToList();
        }
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbset;

            query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            return query.FirstOrDefault();

        }

        public void Add(T item)
        {
           dbset.Add(item);
        }
     
        public bool ObjectExist(Expression<Func<T, bool>> filter)
        {
            return dbset.Any(filter);
        }
        public void Remove(T item)
        {
            dbset.Remove(item);
        }
    }
}