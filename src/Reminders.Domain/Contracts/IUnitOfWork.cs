using System;

namespace Reminders.Domain.Contracts
{
    public interface IUnitOfWork
        : IDisposable
    {
        bool Commit();
        T GetContext<T>() where T : class;
    }

    public interface IUnitOfWork<T>
        : IUnitOfWork
        where T : class
    { }
}
