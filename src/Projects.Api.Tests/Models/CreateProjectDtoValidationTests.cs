using System.ComponentModel.DataAnnotations;
using Projects.Api.Models;

namespace Projects.Api.Tests.Models;

public class CreateProjectDtoValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_IsNullEmptyOrWhitespace_ReturnsValidationError(string? name)
    {
        // Arrange
        var dto = new CreateProjectDto
        {
            Name = name ?? string.Empty,
            Description = "Valid description",
        };

        // Act
        bool isValid = TryValidate(dto, out var validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            r => r.MemberNames.Contains(nameof(CreateProjectDto.Name))
        );
    }

    [Fact]
    public void Name_WithMoreThan100Characters_ReturnsValidationError()
    {
        // Arrange
        var dto = new CreateProjectDto
        {
            Name = new string('a', 101),
            Description = "Valid description",
        };

        // Act
        bool isValid = TryValidate(dto, out var validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            r => r.MemberNames.Contains(nameof(CreateProjectDto.Name))
        );
    }

    [Fact]
    public void Description_WithMoreThan500Characters_ReturnsValidationError()
    {
        // Arrange
        var dto = new CreateProjectDto { Name = "Valid name", Description = new string('b', 501) };

        // Act
        bool isValid = TryValidate(dto, out var validationResults);

        // Assert
        Assert.False(isValid);
        Assert.Contains(
            validationResults,
            r => r.MemberNames.Contains(nameof(CreateProjectDto.Description))
        );
    }

    [Fact]
    public void NameAndDescription_WithValidLengths_ReturnValidationSuccess()
    {
        // Arrange
        var dto = new CreateProjectDto
        {
            Name = "Valid project name",
            Description = new string('c', 500),
        };

        // Act
        bool isValid = TryValidate(dto, out var validationResults);

        // Assert
        Assert.True(isValid);
        Assert.Empty(validationResults);
    }

    private static bool TryValidate(
        CreateProjectDto dto,
        out List<ValidationResult> validationResults
    )
    {
        var context = new ValidationContext(dto);
        validationResults = [];
        return Validator.TryValidateObject(
            dto,
            context,
            validationResults,
            validateAllProperties: true
        );
    }
}
