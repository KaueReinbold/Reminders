using Microsoft.EntityFrameworkCore;
using Reminders.Business.Contracts.Entity;
using Reminders.Business.RepositoryEntities.Persistence.Repositories;
using Reminders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.Persistence.Repositories
{
    public class ReminderRepository : EntityRepositoryGeneric<ReminderEntity>, IReminderRepository
    {
        public ReminderRepository(DbContext context) 
            : base(context)
        {
        }
    }
}
