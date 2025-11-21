using System.Text.RegularExpressions;
using pokedex.Api.Domain.Models;
using pokedex.Api.Infrastructure.Models;

namespace Pokedex.Api.Infrastructure.Mapper;

public static partial  class PokemonMapper
{
    public static PokemonInfoDto ToInfoDto(PokemonInfoApiModel model)
    {
        var englishEntry = model.FlavorTextEntries?
            .FirstOrDefault( e => e.Language.Name == "en");

        return new PokemonInfoDto
        {
            Name = model.Name,
            Description = englishEntry != null ? CatchWhitespaceRegex().Replace(englishEntry.FlavorText, " ").Trim() : "No english description available",
            Habitat = !string.IsNullOrWhiteSpace(model.Habitat?.Name) ? model.Habitat.Name : "Pokemon habitat is unknown",
            IsLegendary = model.IsLegendary
        };
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex CatchWhitespaceRegex();
}