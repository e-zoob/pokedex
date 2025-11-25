using FluentValidation;

namespace pokedex.Api.Validation;

public class PokemonNameValidator: AbstractValidator<string>, IPokemonNameValidator
{
    public PokemonNameValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .Matches("^[a-zA-Z]+$").WithMessage("Name must contain only letters.")
            .Length(3,20).WithMessage("Name length must be between 3 and 20 characters.");
    }   
}