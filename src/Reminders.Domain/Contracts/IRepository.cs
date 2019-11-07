using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminders.Domain.Contracts
{
    public interface IRepository<TEntity>
        : IDisposable
        where TEntity : class
    {
        void Add(TEntity obj);
        void Update(TEntity obj);
        void Remove(TEntity obj);

        int SaveChanges();

        TEntity Get(int id);

        IQueryable<TEntity> Get();
    }
}
