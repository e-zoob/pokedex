using System.Text.Json;
using Pokedex.Api.Domain.Models;
using Pokedex.Api.Infrastructure.Clients;

namespace Pokedex.Api.Domain.Services;

public class TranslationApiService(ITranslationClient client) : ITranslationApiService
{
    public async Task<string> TranslateAsync(string originalText, string style)
    {
        var response = await client.TranslateAsync(originalText, style);

        if (response is null || !response.IsSuccessStatusCode)
            return originalText;

        try
        {
            var json = await response.Content.ReadAsStringAsync();

            var model = JsonSerializer.Deserialize<FunTranslationResponse>(json);

            return model?.Contents?.Translated ?? originalText;
        }
        catch
        {
            return originalText;
        }
    }
}
