using System;
using System.Collections.Generic;
using System.Text;

namespace Reminders.Business.Contracts
{
    public interface IBusinessModelGeneric<T> where T : class
    {
        T Insert(T model);
        bool Update(T model);
        bool Delete(int key);
        List<T> GetAll();
        T Find(int key);
    }
}
