using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reminders.Infrastructure.Data.EntityFramework.Configurations;
using Reminders.Infrastructure.Data.Extensions;
using System;
using System.IO;

namespace Reminders.Infrastructure.Data.EntityFramework.Contexts
{
    public class RemindersContext
        : DbContext
    {
        public RemindersContext() { }

        public RemindersContext(DbContextOptions<RemindersContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new RemindersConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
                optionsBuilder.ConfigureSqlServer();
        }
    }
}