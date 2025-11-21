using System.Text.Json;
using Microsoft.Extensions.Options;
using pokedex.Api.Infrastructure.Models;
using pokedex.Api.Options;

namespace pokedex.Api.Infrastructure.Clients;

public class PokemonInfoClient(HttpClient httpClient, IOptions<PokemonApiOptions> options ) : IPokemonInfoClient
{
        private readonly HttpClient HttpClient = httpClient;
        private readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    public async Task<PokemonInfoApiModel?> GetPokemonInfoAsync(string pokemonName)
    {
        HttpClient.BaseAddress ??= new Uri(options.Value.BaseUri);

        var response = await HttpClient.GetAsync($"pokemon-species/{pokemonName}");

        if ( response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
            
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PokemonInfoApiModel>(JsonOptions);
    }
}