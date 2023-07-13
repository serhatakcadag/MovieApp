using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.data.Abstract
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
