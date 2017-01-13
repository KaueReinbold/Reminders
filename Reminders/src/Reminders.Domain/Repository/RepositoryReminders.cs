
using Microsoft.EntityFrameworkCore;
using Reminders.Data.Context;
using Reminders.Domain.Contract;
using System.Collections.Generic;

namespace Reminders.Domain.Repository
{
    public class RepositoryReminders<T> : IRepositoryReminders<T> where T : class
    {

        private RemindersDbContext _context;

        public RepositoryReminders(RemindersDbContext context)
        {
            _context = context;
        }

        public void Insert(T entity)
        {
            using (_context)
            {
                _context.Set<T>().Add(entity);
                _context.SaveChanges();
            }
        }

        public void Update(T entity)
        {
            using (_context)
            {
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void Delete(T entity)
        {
            using (_context)
            {
                _context.Entry(entity).State = EntityState.Deleted;
                _context.SaveChanges();
            }
        }
        public T Find(int key)
        {
            return _context.Set<T>().Find(key);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }
    }
}
