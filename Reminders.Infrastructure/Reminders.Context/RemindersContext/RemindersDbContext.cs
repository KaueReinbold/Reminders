using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Context.RemindersContext
{
    public class RemindersDbContext : DbContext
    {
        public RemindersDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ReminderEntity> Reminders { get; set; }
    }
}
