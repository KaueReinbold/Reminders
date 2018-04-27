using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminders.Business.Contracts
{
    public interface IRepositoryEntityGeneric<T> where T : class
    {
        T Insert(T entity);
        bool Update(T entity);
        bool Delete(T entity);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Func<T, bool> func);
        T Find(int key);
    }
}
