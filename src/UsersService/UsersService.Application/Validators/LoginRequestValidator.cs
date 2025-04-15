using UsersService.Application.DTO;
using FluentValidation;

namespace UsersService.Application.Validators
{
    public class LoginRequestValidator: AbstractValidator<LoginRequestCustom>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid emailaddress format");
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long");
        }
    }   
}
