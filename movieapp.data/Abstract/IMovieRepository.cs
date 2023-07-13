using System;
using System.Collections.Generic;
using System.Text;
using movieapp.entity;

namespace movieapp.data.Abstract
{
    public interface IMovieRepository : IRepository<Movie>
    {
        //fluent validation
    }
}
