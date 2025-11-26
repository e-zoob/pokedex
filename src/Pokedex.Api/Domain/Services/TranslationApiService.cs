using System.Text.Json;
using Pokedex.Api.Domain.Models;
using Pokedex.Api.Infrastructure.Clients;

namespace Pokedex.Api.Domain.Services;

public class TranslationApiService(ITranslationClient client, ILogger<TranslationApiService> logger) : ITranslationApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<string> TranslateAsync(string originalText, string style, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Translating text using style {Style}", style);

        var response = await client.TranslateAsync(originalText, style, cancellationToken);

        if (response is null || !response.IsSuccessStatusCode)
            return originalText;

        try
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var model = JsonSerializer.Deserialize<FunTranslationResponse>(json, JsonOptions);

            logger.LogDebug("Translation result: {TranslatedText}", model?.Contents?.Translated);
            return model?.Contents?.Translated ?? originalText;
        }
        catch
        {
            logger.LogInformation("Failed to deserialize translation response");
            return originalText;
        }
    }
}
