using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.Contracts
{
    public interface IRepositoryEntityGeneric<T> where T : class
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
        T Find(int key);
    }
}
