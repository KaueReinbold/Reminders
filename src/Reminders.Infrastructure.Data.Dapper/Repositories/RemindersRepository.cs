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
        private IDbConnection dbConnection;

        public RemindersRepository(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public Reminder Add(Reminder obj)
        {
            var query = @"INSERT INTO Reminders SELECT @Id, @Title, @Description, @LimitDate, @IsDone";

            dbConnection.Query(query, obj);

            return Get(obj.Id);
        }

        public bool Exists(Guid id) =>
            !(Get(id) is null);

        public Reminder Get(Guid id) =>
            dbConnection.QueryFirstOrDefault<Reminder>("SELECT * FROM Reminders WHERE Id = @id", new { id });

        public IQueryable<Reminder> Get() =>
            dbConnection.Query<Reminder>("SELECT * FROM Reminders").AsQueryable();

        public void Remove(Guid id) =>
            dbConnection.Execute("DELETE FROM Reminders WHERE Id = @id", new { id });

        public Reminder Update(Reminder obj)
        {
            dbConnection.Query<Reminder>("UPDATE Reminders SET Title = @Title, Description = @Description, LimitDate = @LimitDate, IsDone = @IsDone WHERE Id = @id", obj);

            return Get(obj.Id);
        }
    }
}