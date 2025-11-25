using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pokedex.Api.Domain.Models;
using pokedex.Api.Domain.Services;
using pokedex.Api.Validation;
using Pokedex.Api.Domain.Services;

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
            logger.LogDebug("GetPokemon endpoint called for {Name}", name);

            return await pokemonService.GetPokemonInfoAsync(name);

        })
        .WithName("GetPokemon");

        group.MapGet("/translated/{name}", async Task<Results<
            Ok<PokemonInfoDto>,
            BadRequest<ProblemDetails>,
            NotFound<ProblemDetails>>>

        (
            string name,
            IPokemonApiService pokemonService,
            IPokemonNameValidator validator,
            ITranslationApiService translationService,
            HttpContext httpContext,
            ILogger<Program> logger
        ) =>
        {
            logger.LogDebug("GetPokemonTranslated endpoint called for {Name}", name);

            return await pokemonService.GetTranslatedPokemonInfoAsync(name);
        })
        .WithName("GetPokemonTranslated");

        return app;
    }
}
