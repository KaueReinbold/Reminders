using Microsoft.EntityFrameworkCore;
using Reminders.Domain.Contracts;
using System;

namespace Reminders.Infrastructure.Data.EntityFramework
{
  public class UnitOfWork<TContext>
      : IUnitOfWork<TContext>
      where TContext : DbContext
  {
    private readonly TContext context;
    private bool disposed;

    public UnitOfWork(TContext context) => this.context = context;

    public bool Commit() => context.SaveChanges() > 0;

    public T GetContext<T>() where T : class => context as T;

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposed) return;

      if (disposing) context.Dispose();

      disposed = true;
    }
  }
}
