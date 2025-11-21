namespace pokedex.Api.Domain.Models;

public record PokemonInfoDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string Habitat { get; init; }
    public bool IsLegendary { get; init; }
}