using System.Collections.Generic;
using System.Threading.Tasks;

namespace sup1.data.Abstract
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}