using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest(CategoryTestFixture categoryTestFixture)
{
    private readonly CategoryTestFixture _categoryTestFixture = categoryTestFixture;

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var dateTimeAfter = DateTime.Now;

        // Assert
        Assert.NotNull(category);
        Assert.Equal(category.Name, validCategory.Name);
        Assert.Equal(category.Description, validCategory.Description);
        Assert.NotEqual(default(Guid), category.Id);
        Assert.NotEqual(default(DateTime), category.CreatedAt);
        Assert.True(category.CreatedAt > dateTimeBefore);
        Assert.True(category.CreatedAt < dateTimeAfter);
        Assert.True(category.IsActive);
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActiveStatus))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActiveStatus(bool isActive)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var dateTimeBefore = DateTime.Now;

        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var dateTimeAfter = DateTime.Now;

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.IsActive.Should().Be(isActive);
        category.Id.Should().NotBe(default(Guid));
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt > dateTimeBefore).Should().BeTrue();
        (category.CreatedAt <= dateTimeAfter).Should().BeTrue();
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void ThrowWhenNameIsEmpty(string? name)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(name!, validCategory.Description);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(ThrowWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void ThrowWhenDescriptionIsNull()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(validCategory.Name, null!);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }

    [Theory(DisplayName = nameof(ThrowWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("as")]
    public void ThrowWhenNameIsLessThan3Characters(string name)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(name, validCategory.Description);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(ThrowWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void ThrowWhenNameIsGreaterThan255Characters()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidData = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(invalidData, validCategory.Description);

        // Act
        var exception = Assert.Throws<EntityValidationException>(action);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(ThrowWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void ThrowWhenDescriptionIsGreaterThan10_000Characters()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var invalidData = String.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        Action action = () => new DomainEntity.Category(validCategory.Name, invalidData);

        // Act
        var exception = Assert.Throws<EntityValidationException>(action);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10_000 characters long");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Activate()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);

        // Act
        category.Activate();

        // Assert
        category.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Deactivate()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        // Act
        validCategory.Deactivate();

        // Assert
        validCategory.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "Category - Aggregates")]
    public void Update()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        // Act
        var newValues = new
        {
            Name = "Category updated Name",
            Description = "Category updated Description"
        };
        validCategory.Update(newValues.Name, newValues.Description);

        // Assert
        validCategory.Name.Should().Be(newValues.Name);
        validCategory.Description.Should().Be(newValues.Description);
    }

    [Fact(DisplayName = nameof(UpdateOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateOnlyName()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        // Act
        var newName = "Category Updated Name";
        validCategory.Update(newName);

        // Assert
        validCategory.Name.Should().Be(newName);
        validCategory.Description.Should().Be("Category Description");
    }

    [Theory(DisplayName = nameof(UpdateThrowWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateThrowWhenNameIsEmpty(string? name)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => validCategory.Update(name!, "Category Description");

        // Act & Assert
        action.Should()
           .Throw<EntityValidationException>()
           .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(UpdateThrowWhenILessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    public void UpdateThrowWhenILessThan3Characters(string name)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => validCategory.Update(name);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    [Fact(DisplayName = nameof(UpdateThrowWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateThrowWhenNameIsGreaterThan255Characters()
    {
        // Arrange
        var invalidData = String.Join(null, Enumerable.Range(0, 256).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => validCategory.Update(invalidData);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long");
    }

    [Fact(DisplayName = nameof(UpdateThrowWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateThrowWhenDescriptionIsGreaterThan10_000Characters()
    {
        // Arrange
        var invalidData = String.Join(null, Enumerable.Range(0, 10_001).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => validCategory.Update("Category Name", invalidData);

        // Act & Assert
        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10_000 characters long");
    }
}

