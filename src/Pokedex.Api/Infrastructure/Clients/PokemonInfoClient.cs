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
        private readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    public async Task<PokemonInfoApiModel?> GetPokemonInfoAsync(string pokemonName)
    {
        httpClient.BaseAddress ??= new Uri(options.Value.BaseUri);

        var response = await httpClient.GetAsync($"{pokemonName}");

        if ( response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogInformation("External API returned 404 for {Name}", pokemonName);

            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PokemonInfoApiModel>(JsonOptions);
    }
}