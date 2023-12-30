using System;

namespace Reminders.Domain.Contracts
{
    public interface IUnitOfWork
        : IDisposable
    {
        bool Commit();
        TContext GetContext<TContext>() where TContext : class;
    }

    public interface IUnitOfWork<TContext>
        : IUnitOfWork
        where TContext : class
    { }
}
