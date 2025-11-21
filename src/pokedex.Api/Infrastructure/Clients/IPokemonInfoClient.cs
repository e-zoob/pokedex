using pokedex.Api.Infrastructure.Models;

namespace pokedex.Api.Infrastructure.Clients;

public interface IPokemonInfoClient
{
    Task<PokemonInfoApiModel?> GetPokemonInfoAsync(string pokemonName);
}

