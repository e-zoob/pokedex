
using FluentValidation.Results;

namespace pokedex.Api.Validation;

public interface IPokemonNameValidator
{
    ValidationResult Validate(string name);
}