using System;
using System.Linq;

namespace Reminders.Domain.Contracts
{
    public interface IRepository<T>
        where T : class
    {
        T Add(T obj);
        T Update(T obj);
        void Remove(Guid id);

        T Get(Guid id);

        IQueryable<T> Get();

        bool Exists(Guid id);
    }
}
