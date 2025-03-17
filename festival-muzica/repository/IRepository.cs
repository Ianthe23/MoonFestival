public interface IRepository<TId, T> where T : Entity<TId>
{
    T? FindOne(TId id);
    IEnumerable<T> FindAll();
    T? Save(T entity);
    T? Delete(TId id);
    T? Update(T entity);
}