using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Reminders.Domain.Contracts.Repositories;
using Reminders.Domain.Models;

namespace Reminders.Infrastructure.Data.Dapper.Repositories
{
    public class RemindersRepository
        : IRemindersRepository
    {
        private IDbConnection connection;

        public RemindersRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Reminder Add(Reminder reminder)
        {
            var query = @"INSERT INTO Reminders SELECT @Id, @Title, @Description, @LimitDate, @IsDone";

            connection.Query(query, reminder);

            return reminder;
        }

        public bool Exists(Guid id)
        {
            var reminder = Get(id);

            var reminderExists = reminder is null;

            return reminderExists;
        }

        public Reminder Get(Guid id)
        {
            var query = "SELECT * FROM Reminders WHERE Id = @id";
            var parameters = new { id };

            var reminder = connection.QueryFirstOrDefault<Reminder>(query, parameters);

            return reminder;
        }

        public IQueryable<Reminder> Get()
        {
            var query = "SELECT * FROM Reminders";

            var reminder = connection.Query<Reminder>(query);

            return reminder.AsQueryable();
        }

        public void Remove(Guid id)
        {
            var query = "DELETE FROM Reminders WHERE Id = @id";
            var parameters = new { id };

            connection.Execute(query, parameters);
        }

        public Reminder Update(Reminder reminder)
        {
            var query = "UPDATE Reminders SET Title = @Title, Description = @Description, LimitDate = @LimitDate, IsDone = @IsDone WHERE Id = @id";

            connection.Query<Reminder>(query, reminder);

            return reminder;
        }
    }
}