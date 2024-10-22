﻿using System.Linq.Expressions;

namespace MyMovies.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T item);
         bool ObjectExist(Expression<Func<T, bool>> filter);
        void Remove(T entity);

    }
}
