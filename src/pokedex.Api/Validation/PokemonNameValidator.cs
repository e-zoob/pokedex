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
            .Matches("^[a-zA-Z]+$")
            .WithMessage("Name must contain only letters.");
        
        RuleFor(x => x)
            .Length(3,20)
            .WithMessage("Name length must be between 3 and 20 characters.");
    }   
}