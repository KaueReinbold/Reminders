using System;
using System.Collections.Generic;

namespace Reminders.Domain.Contract
{

    public interface IRepositoryReminders<T> 
    {

        IEnumerable<T> GetAll();

        T Find(int key);

        void Update(T obj);

        void Insert(T obj);

        void Delete(Func<T, bool> predicate);
    }

}
