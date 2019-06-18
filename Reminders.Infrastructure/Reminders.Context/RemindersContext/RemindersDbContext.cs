using Microsoft.EntityFrameworkCore;
using Reminders.Domain.Entities;

namespace Reminders.Context.RemindersContext
{
    public class RemindersDbContext : DbContext
    {
        public RemindersDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ReminderEntity> Reminders { get; set; }
    }
}
