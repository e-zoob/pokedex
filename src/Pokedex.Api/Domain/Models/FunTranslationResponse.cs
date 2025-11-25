namespace Pokedex.Api.Domain.Models;

public class FunTranslationResponse
{
    public FunTranslationContents? Contents { get; set; }
}

public class FunTranslationContents
{
    public string? Translated { get; set; }
}