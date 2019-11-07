using Microsoft.EntityFrameworkCore;
using Reminders.Domain.Contracts;
using System;
using System.Linq;

namespace Reminders.Infrastructure.Data.EntityFramework
{
    public class Repository<TEntity>
        : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> DbSet;
        private bool disposed;

        public Repository(IUnitOfWork unitOfWork)
        {
            Context = unitOfWork.GetContext<DbContext>();
            DbSet = Context.Set<TEntity>();
        }

        public virtual void Add(TEntity obj) => DbSet.Add(obj);
        public virtual void Update(TEntity obj) => DbSet.Update(obj);
        public virtual void Remove(int id) => DbSet.Remove(DbSet.Find(id));

        public int SaveChanges() => Context.SaveChanges();

        public virtual TEntity Get(int id) => DbSet.Find(id);
        public virtual IQueryable<TEntity> Get() => DbSet;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing) Context.Dispose();

            disposed = true;
        }
    }
}