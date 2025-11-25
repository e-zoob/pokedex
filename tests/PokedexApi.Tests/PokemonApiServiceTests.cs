using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using pokedex.Api.Domain.Models;
using pokedex.Api.Domain.Services;
using pokedex.Api.Infrastructure.Clients;
using pokedex.Api.Infrastructure.Models;
using pokedex.Api.Validation;
using Pokedex.Api.Domain.Services;

namespace PokedexApi.Tests
{
    public class PokemonApiServiceTests
    {
        private readonly Mock<IPokemonInfoClient> clientMock = new();
        private readonly Mock<IPokemonNameValidator> validatorMock = new();
        private readonly Mock<ITranslationApiService> translationServiceMock= new();
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<PokemonApiService> logger = NullLogger<PokemonApiService>.Instance;
        public PokemonApiServiceTests()
        {
            memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        
        private PokemonApiService CreateService() => new(clientMock.Object, validatorMock.Object, translationServiceMock.Object, memoryCache, logger);


        [Fact]
        public async Task GetPokemonAsync_InvalidName_ReturnsBadRequest()
        {
            // Arrange
            var name = "Invalid!";
            var failures = new List<ValidationFailure>
        {
            new("name", "Name contains invalid characters.")
        };

            validatorMock
                .Setup(v => v.Validate(name))
                .Returns(new ValidationResult(failures));

            var service = CreateService();

            // Act
            var result = await service.GetPokemonInfoAsync(name);

            // Assert
            var problemDetails = result.Result switch
            {
                BadRequest<ProblemDetails> badRequest => badRequest.Value,
                _ => null
            };

            problemDetails.Should().NotBeNull();
            problemDetails.Title.Should().Be("Invalid Pokémon Name");
            problemDetails.Detail.Should().Contain("invalid");
        }

        [Fact]
        public async Task GetPokemonAsync_NotFound_ReturnsNotFound()
        {
            // Arrange
            var name = "missingpokemon";

            validatorMock.Setup(v => v.Validate(name))
                .Returns(new ValidationResult());

            clientMock.Setup(c => c.GetPokemonInfoAsync(name))
                .ReturnsAsync((PokemonInfoApiModel?)null);

            var service = CreateService();

            // Act
            var result = await service.GetPokemonInfoAsync(name);

            // Assert
            var problemDetails = result.Result switch
            {
                NotFound<ProblemDetails> notFound => notFound.Value,
                _ => null
            };

            problemDetails.Should().NotBeNull();
            problemDetails.Title.Should().Be("Pokemon Not Found");
        }

        [Fact]
        public async Task GetPokemonAsync_ValidName_ReturnsOk()
        {
            // Arrange
            var name = "mewtwo";

            validatorMock.Setup(v => v.Validate(name))
                .Returns(new ValidationResult());

            var apiModel = new PokemonInfoApiModel
            {
                Name = name,
                Habitat = new Habitat { Name = "rare" },
                IsLegendary = true,
                FlavorTextEntries =
                [
                    new FlavorTextEntry { FlavorText = "A Pokémon.", Language = new Language { Name = "en" } }
                ]
            };

            clientMock.Setup(c => c.GetPokemonInfoAsync(name))
                .ReturnsAsync(apiModel);

            var service = CreateService();

            // Act
            var result = await service.GetPokemonInfoAsync(name);

            // Assert
            var pokemonInfo = result.Result switch
            {
                Ok<PokemonInfoDto> okResult => okResult.Value,
                _ => null
            };

            pokemonInfo.Should().NotBeNull();
            pokemonInfo.Should().BeOfType<PokemonInfoDto>();
            pokemonInfo.Description.Should().Be("A Pokémon.");
            pokemonInfo.Name.Should().Be(name);
            pokemonInfo.Habitat.Should().Be("rare");
            pokemonInfo.IsLegendary.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetPokemonInfoAsync_WhenCached_ReturnsCachedValue()
        {
            // Arrange
            var pokemonName = "pikachu";


            validatorMock.Setup(v => v.Validate(pokemonName))
                .Returns(new ValidationResult());

            var cachedDto = new PokemonInfoDto
            {
                Name = "pikachu",
                Habitat = "forest",
                IsLegendary = false,
                Description = "Cached description"
            };

            memoryCache.Set(pokemonName, cachedDto);

            var service = CreateService();

            // Act
            var result = await service.GetPokemonInfoAsync(pokemonName);

            // Assert
            var pokemonInfo = result.Result switch
            {
                Ok<PokemonInfoDto> okResult => okResult.Value,
                _ => null
            };

            pokemonInfo.Should().NotBeNull();
            pokemonInfo.Should().BeOfType<PokemonInfoDto>();

            clientMock.Verify(c => c.GetPokemonInfoAsync(It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task GetPokemonInfoAsync_WhenNotCached_CachesValue()
        {
            // Arrange
            var pokemonName = "pikachu";
            var apiModel = new PokemonInfoApiModel
            {
                Name = "pikachu",
                Habitat = new Habitat { Name = "forest" },
                IsLegendary = false,
                FlavorTextEntries =
                [
                    new() {
                        FlavorText = "Test description",
                        Language = new Language { Name = "en" }
                    }
                ]
            };

            clientMock.Setup(c => c.GetPokemonInfoAsync(pokemonName))
                    .ReturnsAsync(apiModel);

            validatorMock.Setup(v => v.Validate(pokemonName))
                        .Returns(new ValidationResult());

            var service = CreateService();

            // Act
            var result1 = await service.GetPokemonInfoAsync(pokemonName);
            var result2 = await service.GetPokemonInfoAsync(pokemonName);


            var pokemonInfo1 = result1.Result switch
            {
                Ok<PokemonInfoDto> okResult => okResult.Value,
                _ => null
            };


            var pokemonInfo2 = result1.Result switch
            {
                Ok<PokemonInfoDto> okResult => okResult.Value,
                _ => null
            };
        
            pokemonInfo1.Should().NotBeNull();
            pokemonInfo1.Should().BeOfType<PokemonInfoDto>();

            pokemonInfo2.Should().NotBeNull();
            pokemonInfo2.Should().BeOfType<PokemonInfoDto>();

            clientMock.Verify(c => c.GetPokemonInfoAsync(pokemonName), Times.Once);
        }


    }

}