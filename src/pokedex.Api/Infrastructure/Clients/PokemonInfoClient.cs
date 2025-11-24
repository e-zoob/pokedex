using System.Text.Json;
using Microsoft.Extensions.Options;
using pokedex.Api.Infrastructure.Models;
using pokedex.Api.Options;

namespace pokedex.Api.Infrastructure.Clients;

public class PokemonInfoClient(
    HttpClient httpClient, 
    IOptions<PokemonApiOptions> options,
    ILogger<PokemonInfoClient> logger
    ) : IPokemonInfoClient
{
        private readonly HttpClient HttpClient = httpClient;
        private readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    public async Task<PokemonInfoApiModel?> GetPokemonInfoAsync(string pokemonName)
    {
        HttpClient.BaseAddress ??= new Uri(options.Value.BaseUri);

        logger.LogInformation("Fetching info for Pokémon: {PokemonName}", pokemonName);
        var response = await HttpClient.GetAsync($"pokemon-species/{pokemonName}");

        if ( response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogInformation("External API returned 404 for {Name}", pokemonName);

            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PokemonInfoApiModel>(JsonOptions);
    }
}