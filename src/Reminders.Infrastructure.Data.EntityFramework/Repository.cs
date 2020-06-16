using Microsoft.EntityFrameworkCore;
using Reminders.Domain.Contracts;
using Reminders.Domain.Models;
using System;
using System.Linq;

namespace Reminders.Infrastructure.Data.EntityFramework
{
  public class Repository<TEntity>
      : IRepository<TEntity>
      where TEntity : Entity<Guid>
  {
    protected readonly DbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    private bool disposed;

    public Repository(IUnitOfWork unitOfWork)
    {
      Context = unitOfWork.GetContext<DbContext>();
      DbSet = Context.Set<TEntity>();
    }

    public virtual TEntity Add(TEntity obj) =>
        DbSet.Add(obj).Entity;
    public virtual TEntity Update(TEntity obj) =>
        DbSet.Update(obj).Entity;
    public virtual void Remove(Guid id) =>
        DbSet.Remove(DbSet.Find(id));

    public virtual TEntity Get(Guid id) =>
        DbSet.Find(id);
    public virtual TEntity GetAsNoTracking(Guid id)
    {
      var entity = Context.Set<TEntity>().Find(id);
      Context.Entry(entity).State = EntityState.Detached;
      return entity;
    }

    public virtual IQueryable<TEntity> Get() =>
        DbSet;
    public virtual IQueryable<TEntity> GetAsNoTracking() =>
        DbSet.AsNoTracking();

    public bool Exists(Guid id)
    {
      var entity = GetAsNoTracking(id);

      if (entity is null)
        return false;

      return !entity.IsDeleted;
    }

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