using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using pokedex.Api.Domain.Models;
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

        private PokemonApiService CreateService() => new(clientMock.Object, validatorMock.Object);


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
    }

}