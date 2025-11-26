using System.Net;
using System.Text;
using Moq;
using Pokedex.Api.Domain.Services;
using Pokedex.Api.Infrastructure.Clients;

namespace PokedexApi.Tests;

public class TranslationApiServiceTests
{
    private readonly Mock<ITranslationClient> clientMock = new();
    private readonly TranslationApiService service;

    public TranslationApiServiceTests()
    {
        service = new TranslationApiService(clientMock.Object);
    }

    [Fact]
    public async Task TranslateAsync_WhenSuccess_ReturnsTranslatedText()
    {
        // Arrange
        string original = "hello";
        string translated = "Force be with you";

        var json = $$"""
    {
        "contents": {
            "translated": "{{translated}}"
        }
    }
    """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        clientMock
            .Setup(c => c.TranslateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        // Act
        var result = await service.TranslateAsync(original, "yoda");

        // Assert
        clientMock.Verify(c => c.TranslateAsync(original, "yoda"), Times.Once);
        Assert.Equal(translated, result);
    }

    [Fact]
    public async Task TranslateAsync_WhenResponseIsNull_ReturnsOriginal()
    {
        clientMock
            .Setup(c => c.TranslateAsync("hello", "yoda"))
            .ReturnsAsync((HttpResponseMessage?)null);

        var result = await service.TranslateAsync("hello", "yoda");

        Assert.Equal("hello", result);
    }

    [Fact]
    public async Task TranslateAsync_WhenResponseIsNotSuccess_ReturnsOriginal()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

        clientMock
            .Setup(c => c.TranslateAsync("hello", "yoda"))
            .ReturnsAsync(response);

        var result = await service.TranslateAsync("hello", "yoda");

        Assert.Equal("hello", result);
    }

    [Fact]
    public async Task TranslateAsync_WhenJsonMalformed_ReturnsOriginal()
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("THIS IS NOT JSON")
        };

        clientMock
            .Setup(c => c.TranslateAsync("hello", "yoda"))
            .ReturnsAsync(response);

        var result = await service.TranslateAsync("hello", "yoda");

        Assert.Equal("hello", result);
    }
}