using FluentAssertions;
using pokedex.Api.Infrastructure.Models;
using Pokedex.Api.Infrastructure.Mapper;

namespace PokedexApi.Tests;

public class PokemonMapperTests
{

    private readonly PokemonMapper Mapper;

    public PokemonMapperTests()
    {
        Mapper = new PokemonMapper();
    }

    [Fact]
    public void ToInfoDto_WithCompleteData_MapsAllFieldsCorrectly()
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "pikachu",
            FlavorTextEntries =
            [
                new()
                {
                    FlavorText = "When several of these POKéMON gather, their electricity could build and cause lightning storms.",
                    Language = new Language { Name = "en" }
                }
            ],
            Habitat = new Habitat { Name = "forest" },
            IsLegendary = false
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("pikachu");
        result.Description.Should().Be("When several of these POKéMON gather, their electricity could build and cause lightning storms.");
        result.Habitat.Should().Be("forest");
        result.IsLegendary.Should().BeFalse();
    }

    [Fact]
    public void ToInfoDto_WithNewlineInDescription_ReplacesWithSpace()
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "mewtwo",
            FlavorTextEntries =
            [
                new()
                {
                    FlavorText = "It was created by\na scientist after\nyears of\t horrific\r\t gene\n\n splicing.",
                    Language = new Language { Name = "en" }
                }
            ],
            Habitat = new Habitat { Name = "rare" },
            IsLegendary = true
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Description.Should().Be("It was created by a scientist after years of horrific gene splicing.");
        result.Description.Should().NotContain("\n");
        result.Description.Should().NotContain("\t");
        result.Description.Should().NotContain("\r");
    }

    [Fact]
    public void ToInfoDto_WithMultipleLanguages_SelectsEnglishEntry()
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "charizard",
            FlavorTextEntries =
            [
                new()
                {
                    FlavorText = "Descripción en español",
                    Language = new Language { Name = "es" }
                },
                new()
                {
                    FlavorText = "English description here",
                    Language = new Language { Name = "en" }
                },
                new()
                {
                    FlavorText = "Description française",
                    Language = new Language { Name = "fr" }
                }
            ],
            Habitat = new Habitat { Name = "mountain" },
            IsLegendary = false
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Description.Should().Be("English description here");
    }

    [Fact]
    public void ToInfoDto_WithNoEnglishEntry_ReturnsDefaultMessage()
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "ditto",
            FlavorTextEntries =
            [
                new()
                {
                    FlavorText = "Descripción en español",
                    Language = new Language { Name = "es" }
                },
                new()
                {
                    FlavorText = "日本語の説明",
                    Language = new Language { Name = "ja" }
                }
            ],
            Habitat = new Habitat { Name = "urban" },
            IsLegendary = false
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Description.Should().Be("No english description available");
    }

    [Fact]
    public void ToInfoDto_WithEmptyFlavorTextEntries_ReturnsDefaultMessage()
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "missingno",
            FlavorTextEntries = [],
            Habitat = new Habitat { Name = "unknown" },
            IsLegendary = false
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Description.Should().Be("No english description available");
    }

    [Fact]
    public void ToInfoDto_WithNullFlavorTextEntries_ReturnsDefaultMessage()
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "missingno",
            FlavorTextEntries = null!,
            Habitat = new Habitat { Name = "unknown" },
            IsLegendary = false
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Description.Should().Be("No english description available");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void ToInfoDto_WithEmptyHabitat_ReturnsDefaultHabitatMessage(string? habitat)
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = "voltorb",
            FlavorTextEntries =
            [
                new()
                {
                    FlavorText = "Usually found in power plants.",
                    Language = new Language { Name = "en" }
                }
            ],
            Habitat = new Habitat{ Name = habitat!},
            IsLegendary = false
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.Habitat.Should().Be("Pokemon habitat is unknown");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToInfoDto_MapsIsLegendaryCorrectly(bool isLegendary)
    {
        // Arrange
        var model = new PokemonInfoApiModel
        {
            Name = isLegendary ? "articuno" : "rattata",
            FlavorTextEntries =
            [
                new()
                {
                    FlavorText = "A Pokémon description.",
                    Language = new Language { Name = "en" }
                }
            ],
            Habitat = new Habitat { Name = isLegendary ? "rare" : "grassland" },
            IsLegendary = isLegendary
        };

        // Act
        var result = Mapper.ToInfoDto(model);

        // Assert
        result.IsLegendary.Should().Be(isLegendary);
    }
}

