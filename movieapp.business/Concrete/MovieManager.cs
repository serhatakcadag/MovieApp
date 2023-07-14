using FluentValidation;
using FluentValidation.Results;
using movieapp.business.Abstract;
using movieapp.business.Validator;
using movieapp.data.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace movieapp.business.Concrete
{
    public class MovieManager : IMovieService
    {
        private IMovieRepository movieRepository;

        private IValidator<Movie> movieValidator;

        public MovieManager(IMovieRepository movieRepository, IValidator<Movie> movieValidator)
        {
            this.movieRepository = movieRepository;
            this.movieValidator = movieValidator;
        }
        public async Task Create(Movie entity)
        {
            ValidationResult validationResult = movieValidator.Validate(entity);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await movieRepository.Create(entity);
        }

        public async Task Delete(Movie entity)
        {
            await movieRepository.Delete(entity);
        }

        public async Task<List<Movie>> GetAll()
        {
            return await movieRepository.GetAll();
        }

        public async Task<Movie> GetById(int id)
        {
            return await movieRepository.GetById(id);
        }

        public async Task Update(Movie entity)
        {
            Console.WriteLine(entity.ReleaseDate.ToString());
            ValidationResult validationResult = movieValidator.Validate(entity);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            await movieRepository.Update(entity);
        }
    }
}
