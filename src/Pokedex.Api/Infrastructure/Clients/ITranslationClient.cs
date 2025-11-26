namespace Pokedex.Api.Infrastructure.Clients;

public interface ITranslationClient
{
    Task<HttpResponseMessage?> TranslateAsync(string text, string style, CancellationToken cancellationToken = default); 
}