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
        void Remove(Guid id);

        int SaveChanges();

        TEntity Get(Guid id);

        IQueryable<TEntity> Get();
    }
}
