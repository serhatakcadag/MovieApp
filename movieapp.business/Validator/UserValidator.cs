using FluentValidation;
using movieapp.data.Abstract;
using movieapp.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace movieapp.business.Validator
{
    public class UserValidator : AbstractValidator<UserRegister>
    {
         private /*readonly*/ IUserRepository userRepository;

        public UserValidator(IUserRepository userRepository)
        {
            this.userRepository = userRepository;

            RuleFor(u => u.User.Username).NotEmpty().WithMessage("Username is required.")/*.Custom((username, context) => {
                var users = userRepository.GetAll().Result;
                var user = users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    context.AddFailure("Username is already in use");
                }
            })*/;

            RuleFor(u => u.User.Email)
              .NotEmpty().WithMessage("Email is required.")
              .EmailAddress().WithMessage("Email is not valid.")
             /* .Custom((email, context) => {
                  var users = userRepository.GetAll().Result;
                  var user = users.FirstOrDefault(u => u.Email == email);
                  if (user != null)
                  {
                      context.AddFailure("Email is already in use");
                  }
              })*/;

            RuleFor(u => u.User.Password).Equal(u=> u.PasswordConfirmation).WithMessage("Password confirmation doesn't match with the password")
                .MinimumLength(6).WithMessage("Password cannot be less than 6 characters");
        }
    }
}
