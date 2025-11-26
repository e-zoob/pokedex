namespace Pokedex.Api.Domain.Services;

public interface ITranslationApiService
{
    Task<string> TranslateAsync(string originalText, string style, CancellationToken cancellationToken = default);
}