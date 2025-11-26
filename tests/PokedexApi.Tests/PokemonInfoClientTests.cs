using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using pokedex.Api.Infrastructure.Clients;
using pokedex.Api.Infrastructure.Models;
using pokedex.Api.Options;

namespace PokedexApi.Tests;

public class PokemonInfoClientTests
{
    private readonly Mock<HttpMessageHandler> HandlerMock;
    private readonly HttpClient HttpClient;
    private readonly IOptions<PokemonApiOptions> PokemonApiOptions;
    private readonly ILogger<PokemonInfoClient> logger = NullLogger<PokemonInfoClient>.Instance;
    public PokemonInfoClientTests()
    {
        HandlerMock = new Mock<HttpMessageHandler>();
        HttpClient = new HttpClient(HandlerMock.Object);
        PokemonApiOptions = Options.Create(new PokemonApiOptions 
        { 
                    BaseUri = "https://pokeapi.co/api/v2/pokemon-species/" 
        });

    }

    [Fact]
    public async Task GetPokemonInfoAsync_WithValidPokemon_ReturnsDeserializedModel()
    {
        // Arrange
        var pokemonName = "pikachu";
        var expectedModel = new PokemonInfoApiModel
        {
            Name = "pikachu",
            FlavorTextEntries = 
            [
                new FlavorTextEntry
                {
                    FlavorText = "When several of these Pokémon gather, their electricity could build and cause lightning storms.",
                    Language = new Language { Name = "en" }
                }
            ],
            Habitat = new Habitat { Name = "forest" },
            IsLegendary = false
        };

        var jsonResponse = JsonSerializer.Serialize(expectedModel);
        var httpResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        HandlerMock
            .SetupRequest(HttpMethod.Get, $"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .ReturnsJsonResponse(expectedModel);

        var client = new PokemonInfoClient(HttpClient, PokemonApiOptions, logger);

        // Act
        var result = await client.GetPokemonInfoAsync(pokemonName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("pikachu");
        result.FlavorTextEntries.Should().HaveCount(1);
        result.Habitat!.Name.Should().Be("forest");
        result.IsLegendary.Should().BeFalse();
    }

    [Fact]
    public async Task GetPokemonInfoAsync_WithNullHabitat_ReturnsValidResult()
    {
        // Arrange
        var pokemonName = "mewtwo";
        var responseModel = new PokemonInfoApiModel
        {
            Name = "mewtwo",
            FlavorTextEntries = [],
            Habitat = null,
            IsLegendary = true
        };

        HandlerMock
            .SetupRequest(HttpMethod.Get, $"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .ReturnsJsonResponse(responseModel);

        var client = new PokemonInfoClient(HttpClient, PokemonApiOptions, logger);

        // Act
        var result = await client.GetPokemonInfoAsync(pokemonName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("mewtwo");
        result.Habitat.Should().BeNull();
        result.IsLegendary.Should().BeTrue();
    }

    [Fact]
    public async Task GetPokemonInfoAsync_When404_ReturnNull()
    {
        // Arrange
        var pokemonName = "fakemon";

        HandlerMock
            .SetupRequest(HttpMethod.Get, $"https://pokeapi.co/api/v2/pokemon-species/{pokemonName}")
            .ReturnsResponse(HttpStatusCode.NotFound);

        var client = new PokemonInfoClient(HttpClient, PokemonApiOptions, logger);
        
        // Act
        var result = await client.GetPokemonInfoAsync(pokemonName);

        // Assert
         result.Should().BeNull();
    }
}