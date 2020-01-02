using System;
using System.Linq;

namespace Reminders.Domain.Contracts
{
    public interface IRepository<T>
        where T : class
    {
        void Add(T obj);
        void Update(T obj);
        void Remove(Guid id);

        T Get(Guid id);
        
        IQueryable<T> Get();
    }
}
