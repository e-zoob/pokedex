using FluentAssertions;
using FluentValidation.TestHelper;
using pokedex.Api.Validation;


namespace PokedexApi.Tests;

public class PokemonNameValidatorTests
{
    private readonly PokemonNameValidator validator;

    public PokemonNameValidatorTests()
    {
        validator = new();
    }

    [Theory]
    [InlineData("pikachu")]
    [InlineData("DITTO")]
    [InlineData("aegislash")]
    [InlineData("muk")]
    [InlineData("Crabominable")]
    public void Valid_Name_Shoul_Pass(string name)
    {
        // Act
        var result = validator.TestValidate(name);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    public void Empty_Or_WhiteSpace_Name_Should_Fail(string name)
    {
        // Act
        var result = validator.TestValidate(name);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Name cannot be empty.");
    }


    [Theory]
    [InlineData("p@ikachu")]
    [InlineData("dr feelgood")]
    [InlineData("mewtwo!")]
    public void Invalid_Characters_Should_Fail(string name)
    {
        // Act
        var result = validator.TestValidate(name);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Name must contain only letters.");
    }

    [Theory]
    [InlineData("me")]
    [InlineData("mypokemonnameistoolong")]
    public void Invalid_Length_Should_Fail(string name)
    {
        // Act
        var result = validator.TestValidate(name);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Name length must be between 3 and 20 characters.");
    }
}