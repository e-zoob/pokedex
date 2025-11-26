using System.Text.Json;
using Pokedex.Api.Domain.Models;
using Pokedex.Api.Infrastructure.Clients;

namespace Pokedex.Api.Domain.Services;

public class TranslationApiService(ITranslationClient client) : ITranslationApiService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<string> TranslateAsync(string originalText, string style, CancellationToken cancellationToken = default)
    {
        var response = await client.TranslateAsync(originalText, style, cancellationToken);

        if (response is null || !response.IsSuccessStatusCode)
            return originalText;

        try
        {
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var model = JsonSerializer.Deserialize<FunTranslationResponse>(json, JsonOptions);

            return model?.Contents?.Translated ?? originalText;
        }
        catch
        {
            return originalText;
        }
    }
}
