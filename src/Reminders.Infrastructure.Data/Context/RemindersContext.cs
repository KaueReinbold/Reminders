using Microsoft.EntityFrameworkCore;
using Reminders.Infrastructure.Data.Configuration;

namespace Reminders.Infrastructure.Data.Context
{
    public class RemindersContext
        : DbContext
    {
        public RemindersContext(DbContextOptions<RemindersContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RemindersConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
