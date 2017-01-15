using System.Collections.Generic;

namespace Reminders.Domain.Contract
{
    /// <summary>
    /// Application repository contract.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        T Find(int key);
    }
}
