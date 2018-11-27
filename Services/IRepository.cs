using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task<T> InsertAsyc(T t);

        T Insert(T t);

        Task<int> DeleteAsync(T t);
        int Delete(T t);
    }
}
