using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using pokedex.Api.Domain.Models;
using pokedex.Api.Infrastructure.Clients;
using pokedex.Api.Infrastructure.Models;
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
    public async Task<Results<Ok<PokemonInfoDto>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> GetPokemonInfoAsync(string name, CancellationToken cancellationToken = default)
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

        var pokemonInfo = await GetPokemonInfoModelAsync(name, cancellationToken);

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

    public async Task<Results<Ok<PokemonInfoDto>,BadRequest<ProblemDetails>,NotFound<ProblemDetails>>> GetTranslatedPokemonInfoAsync(string name, CancellationToken cancellationToken = default)
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
        
        var info = await GetPokemonInfoModelAsync(name, cancellationToken);
        if (info is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Pokemon Not Found",
                Detail = $"Pokemon '{name}' does not exist."
            });
        }

        var dto = PokemonMapper.ToInfoDto(info);
        var description = dto.Description;

        if (string.IsNullOrWhiteSpace(description) || description == "No english description available")
        {
            return TypedResults.Ok(dto);
        }

        string style = info.Habitat?.Name?.ToLower() == "cave" || info.IsLegendary
            ? "yodish"
            : "shakespeare-english";
        
        var normalizedName = name.Trim().ToLowerInvariant();

        var translatedCacheKey = $"pokemon-translated:{normalizedName}";
        
        if(cache.TryGetValue(translatedCacheKey, out PokemonInfoDto? cacheTranslated))
        {
            return TypedResults.Ok(cacheTranslated);
        }

        string translated = await translationService.TranslateAsync(description, style, cancellationToken);

        cache.Set(translatedCacheKey, dto, TimeSpan.FromMinutes(5));

        return TypedResults.Ok(dto with {Description = translated});
    }

    private async Task<PokemonInfoApiModel?> GetPokemonInfoModelAsync(string name, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        var cacheKey = $"pokemon-info:{normalizedName}";

        if(cache.TryGetValue(cacheKey, out PokemonInfoApiModel? cached))
        {
            logger.LogDebug("Cache hit for {Name}", normalizedName);
            return cached;
        }

        var pokemonInfo = await client.GetPokemonInfoAsync(normalizedName, cancellationToken);
        if(pokemonInfo is null)
        {
            return null;
        }
        cache.Set(cacheKey, pokemonInfo, TimeSpan.FromMinutes(5));
        return pokemonInfo;
    }
}
