using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using pokedex.Api.Domain.Models;
using pokedex.Api.Infrastructure.Clients;
using pokedex.Api.Validation;
using Pokedex.Api.Domain.Services;
using Pokedex.Api.Infrastructure.Mapper;

namespace pokedex.Api.Domain.Services;

public class PokemonApiService(
    IPokemonInfoClient client, 
    IPokemonNameValidator validator, 
    ITranslationApiService translationService,
    IMemoryCache cache,
    ILogger<PokemonApiService> logger
    ) : IPokemonApiService
{
    public async Task<Results<Ok<PokemonInfoDto>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> GetPokemonInfoAsync(string name)
    {
        logger.LogDebug("Getting pokemon info for {Name}", name);

        var validation = validator.Validate(name);

        if (!validation.IsValid)
        {
            logger.LogInformation("Validation failed for {Name}: {Error}", name, validation.Errors.FirstOrDefault()?.ErrorMessage);

            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid Pokémon Name",
                Detail = validation.Errors.First().ErrorMessage
            });
        }
        
        if (cache.TryGetValue(name, out PokemonInfoDto? cached))
        {
            logger.LogDebug("Cache hit for {Name}", name);
            return TypedResults.Ok(cached);
        }

        var pokemonInfo = await client.GetPokemonInfoAsync(name);

        if(pokemonInfo is null)
        {
            logger.LogInformation("Pokemon not found: {Name}", name);

            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Pokemon Not Found",
                Detail = $"Pokèmon '{name}' does not exist."
            });
        }

        var dto = PokemonMapper.ToInfoDto(pokemonInfo);

        cache.Set(name, dto, TimeSpan.FromMinutes(5));

        return TypedResults.Ok(dto);
    }

    public async Task<Results<
        Ok<PokemonInfoDto>,
        BadRequest<ProblemDetails>,
        NotFound<ProblemDetails>>>
        GetTranslatedPokemonInfoAsync(string name)
    {
        logger.LogDebug("Getting pokemon info for {Name}", name);

        var validation = validator.Validate(name);
        if (!validation.IsValid)
        {
            logger.LogInformation("Validation failed for {Name}: {Error}", name, validation.Errors.FirstOrDefault()?.ErrorMessage);

            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid Pokémon Name",
                Detail = validation.Errors.First().ErrorMessage
            });
        }

        var info = await client.GetPokemonInfoAsync(name);
        if (info is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Pokemon Not Found",
                Detail = $"Pokemon '{name}' does not exist."
            });
        }

        var description = info.FlavorTextEntries
            .FirstOrDefault(e => e.Language?.Name == "en")?
            .FlavorText ?? "";

        if (string.IsNullOrWhiteSpace(description))
        {
            var dtoNoTranslation = PokemonMapper.ToInfoDto(info);
            return TypedResults.Ok(dtoNoTranslation);
        }

        string style = info.Habitat?.Name?.ToLower() == "cave" || info.IsLegendary
            ? "yoda"
            : "shakespeare";

        string translated = await translationService.TranslateAsync(description, style);

        var dto = PokemonMapper.ToInfoDto(info) with { Description = translated };

        return TypedResults.Ok(dto);
    }
}
