using FluentValidation;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace movieapp.business.Validator
{
    public class MovieValidator : AbstractValidator<Movie>
    {
        public MovieValidator()
        {
            RuleFor(m => m.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(m => m.Description).NotEmpty().WithMessage("Description is required")
                .MinimumLength(10).WithMessage("Description field cannot be less than 10 characters"); ;
            RuleFor(m => m.ReleaseDate).NotEmpty().WithMessage("Release date is required.");
        }
    }
}
