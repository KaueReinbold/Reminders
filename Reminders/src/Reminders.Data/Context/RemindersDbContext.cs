using Microsoft.EntityFrameworkCore;
using Reminders.Data.Entity;

namespace Reminders.Data.Context
{
    /// <summary>
    /// Class that implements the Application Context.
    /// </summary>
    public class RemindersDbContext : DbContext
    {
        public RemindersDbContext(DbContextOptions<RemindersDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ReminderEntity> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReminderEntity>().ToTable("tb_reminder");
        }
    }
}
