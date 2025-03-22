using System.Collections.Generic;
using festival_muzica_avalonia.Models;

namespace festival_muzica_avalonia.Repository
{
    public interface IRepository<TId, T> where T : Entity<TId>
    {
        T? FindOne(TId id);
        IEnumerable<T> FindAll();
        T? Save(T entity);
        T? Delete(TId id);
        T? Update(T entity);
    }
}