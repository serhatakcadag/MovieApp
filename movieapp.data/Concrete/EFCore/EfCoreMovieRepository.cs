using movieapp.data.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace movieapp.data.Concrete.EFCore
{
    public class EfCoreMovieRepository : EfCoreGenericRepository<Movie, MovieContext>, IMovieRepository
    {
        public EfCoreMovieRepository(MovieContext context) : base(context)
        {

        }
    }
}
