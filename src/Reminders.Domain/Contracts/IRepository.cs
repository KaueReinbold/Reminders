using System;
using System.Linq;

namespace Reminders.Domain.Contracts
{
    public interface IRepository<TEntity>
        : IDisposable
        where TEntity : class
    {
        void Add(TEntity obj);
        void Update(TEntity obj);
        void Remove(int id);

        int SaveChanges();

        TEntity Get(int id);

        IQueryable<TEntity> Get();
    }
}
