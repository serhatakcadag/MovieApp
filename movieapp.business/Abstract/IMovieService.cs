using movieapp.data.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.business.Abstract
{
    public interface IMovieService
    {
        Task<List<Movie>> GetAll();
        Task<Movie> GetById(int id);
        Task Create(Movie entity);
        Task Update(Movie entity);
        Task Delete(Movie entity);
    }
}
