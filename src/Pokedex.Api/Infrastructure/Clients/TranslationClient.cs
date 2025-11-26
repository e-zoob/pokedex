using System.Text.Json;
using Microsoft.Extensions.Options;
using Pokedex.Api.Options;

namespace Pokedex.Api.Infrastructure.Clients;

public class TranslationClient(
    HttpClient httpClient,
    IOptions<TranslationApiOptions> options
) : ITranslationClient
{
    private readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<HttpResponseMessage?> TranslateAsync(string text, string style, CancellationToken cancellationToken = default)
    {
        httpClient.BaseAddress ??= new Uri(options.Value.BaseUri);

        using var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("text", text)
        ]);

        return await httpClient.PostAsync($"{style}.json", content, cancellationToken);
    }
}