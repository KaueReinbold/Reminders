﻿using Microsoft.EntityFrameworkCore;
using Reminders.Data.Entity;

namespace Reminders.Data.Context
{
    public class RemindersDbContext : DbContext
    {
        public RemindersDbContext(DbContextOptions<RemindersDbContext> options) : base(options) { }

        public DbSet<ReminderEntity> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReminderEntity>().ToTable("tbReminder");
        }
    }
}
