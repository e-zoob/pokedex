using FluentValidation;

namespace pokedex.Api.Validation;

public class PokemonNameValidator: AbstractValidator<string>
{
    public PokemonNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty()
            .WithMessage("Name cannot be empty.");
        
        RuleFor(x => x)
            .Matches("^[a-z]+$")
            .WithMessage("Name must contain only lowercase letters.");
        
        RuleFor(x => x.Length)
            .GreaterThanOrEqualTo(2)
            .LessThanOrEqualTo(20)
            .WithMessage("Name length must be between 2 and 20 characters.");
    }
    
}