using Reminders.Data.Context;
using Reminders.Domain.Contract;
using Reminders.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reminders.Domain.Repository
{
    public class RepositoryReminders<T> : IRepositoryReminders<T>
    {

        private RemindersDbContext _context;

        public RepositoryReminders(RemindersDbContext context)
        {
            _context = context;
        }

        public void Delete(Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public T Find(int key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Insert(T obj)
        {
            throw new NotImplementedException();
        }

        public void Update(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
