
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pokedex.Api.Domain.Models;

namespace pokedex.Api.Domain.Services;

public interface IPokemonApiService
{
    Task<Results<Ok<PokemonInfoDto>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> GetPokemonInfoAsync(string pokemonName);        
}