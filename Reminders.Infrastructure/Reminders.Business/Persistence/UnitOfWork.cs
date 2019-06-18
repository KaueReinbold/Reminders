using Microsoft.EntityFrameworkCore;
using Reminders.Business.Contracts;
using Reminders.Business.Contracts.Entity;
using Reminders.Business.Persistence.Repositories;
using Reminders.Context.RemindersContext;
using System.Linq;

namespace Reminders.Business.RepositoryEntities.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RemindersDbContext _context;

        public IReminderRepository RemindersRepository { get; }

        public UnitOfWork(RemindersDbContext context)
        {
            _context = context;
            RemindersRepository = new ReminderRepository(context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void RejectChanges()
        {
            foreach (var entry in _context.ChangeTracker.Entries()
              .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
