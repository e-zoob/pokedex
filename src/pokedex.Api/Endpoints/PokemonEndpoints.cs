using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pokedex.Api.Domain.Models;
using pokedex.Api.Domain.Services;
using pokedex.Api.Validation;

namespace pokedex.Api.Endpoints;

public static class PokemonEndpoints
{
     public static IEndpointRouteBuilder MapPokemonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/pokemon")
                       .WithTags("Pokemon");

        group.MapGet("/{name}", async Task<Results<
            Ok<PokemonInfoDto>,
            BadRequest<ProblemDetails>,
            NotFound<ProblemDetails>>>
        (
            string name,
            IPokemonNameValidator validator,
            IPokemonApiService pokemonService,
            HttpContext httpContext,
            ILogger<Program> logger
        ) =>
        {
            logger.LogInformation("GetPokemon endpoint called for {Name}", name);

            var result = await pokemonService.GetPokemonInfoAsync(name);

            return result;
        })
        .WithName("GetPokemon");

        return app;
    }
}
