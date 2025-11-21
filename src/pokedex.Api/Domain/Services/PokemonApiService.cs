using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pokedex.Api.Domain.Models;
using pokedex.Api.Domain.Services;
using pokedex.Api.Infrastructure.Clients;
using pokedex.Api.Validation;
using Pokedex.Api.Infrastructure.Mapper;

namespace Pokedex.Api.Domain.Services;

public class PokemonApiService(IPokemonInfoClient client, IPokemonNameValidator validator) : IPokemonApiService
{
    public async Task<Results<Ok<PokemonInfoDto>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> GetPokemonInfoAsync(string name)
    {
        var validation = validator.Validate(name);

        if (!validation.IsValid)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid Pokémon Name",
                Detail = validation.Errors.First().ErrorMessage
            });
        }

        var pokemonInfo = await client.GetPokemonInfoAsync(name);

        if(pokemonInfo is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Pokemon Not Found",
                Detail = $"Pokèmon '{name}' does not exist."
            });
        }

        var dto = PokemonMapper.ToInfoDto(pokemonInfo);
        return TypedResults.Ok(dto);
    }
}
