using System.Text.Json.Serialization;

namespace pokedex.Api.Infrastructure.Models;

public class PokemonInfoApiModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("habitat")]
    public Habitat? Habitat { get; set; }

    [JsonPropertyName("is_legendary")]
    public bool IsLegendary { get; set;  }

    [JsonPropertyName("flavor_text_entries")]
    public List<FlavorTextEntry> FlavorTextEntries { get; set;} = [];
}

public class FlavorTextEntry
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public Language Language { get; set; } = new Language();
}

public class Habitat
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class Language
{
    [JsonPropertyName("name")]
    public string Name { get; set; }  = string.Empty;
}