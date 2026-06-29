namespace CodeMe.ServiceErrors.Abstractions.UnitTests;

public class ErrorUriTests
{
    [Fact]
    public void Equals_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service");
        var otherGroup = ErrorGroupUri.Create("app-error", "test-service-2");
        var error = ErrorUri.Create(group, "not-found");
        var same = ErrorUri.Create(group, "not-found");
        var sameUpperCase = ErrorUri.Create(group, "NOT-FOUND");
        var otherError = ErrorUri.Create(group, "not-found-2");
        var otherGroupError = ErrorUri.Create(otherGroup, "not-found");

        // Assert
        error.Equals(error).Should().BeTrue();
        error.Equals(same).Should().BeTrue();
        error.Equals(sameUpperCase).Should().BeTrue();
        error.Equals(otherError).Should().BeFalse();
        error.Equals(otherGroupError).Should().BeFalse();
        error.Equals(null).Should().BeFalse();

        error!.Equals((object)error).Should().BeTrue();
        error.Equals((object)same).Should().BeTrue();
        error.Equals((object)sameUpperCase).Should().BeTrue();
        error.Equals((object)otherError).Should().BeFalse();
        error.Equals((object)otherGroupError).Should().BeFalse();
        error.Equals((object?)null).Should().BeFalse();

        (error == same).Should().BeTrue();
        (error == otherError).Should().BeFalse();

        (error != same).Should().BeFalse();
        (error != otherError).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service");
        var otherGroup = ErrorGroupUri.Create("app-error", "test-service-2");
        var error = ErrorUri.Create(group, "not-found");
        var same = ErrorUri.Create(group, "not-found");
        var sameUpperCase = ErrorUri.Create(group, "NOT-FOUND");
        var otherError = ErrorUri.Create(group, "not-found-2");
        var otherGroupError = ErrorUri.Create(otherGroup, "not-found");

        // Assert
        error.GetHashCode().Should().Be(same.GetHashCode());
        error.GetHashCode().Should().Be(sameUpperCase.GetHashCode());
        error.GetHashCode().Should().NotBe(otherError.GetHashCode());
        error.GetHashCode().Should().NotBe(otherGroupError.GetHashCode());
    }

    [Fact]
    public void GroupContains_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service");
        var otherGroup = ErrorGroupUri.Create("app-error", "test-service-2");
        var error = ErrorUri.Create(group, "not-found");

        // Assert
        group.Contains(error).Should().BeTrue();
        otherGroup.Contains(error).Should().BeFalse();
    }

    [Fact]
    public void Error_Properties_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("app-error", "test-service");
        var error = ErrorUri.Create(errorGroup, "user-not-found");

        // Assert
        error.Group.Should().Be(errorGroup);
        error.Code.Should().Be("user-not-found");
        error.Uri.ToString().Should().Be("app-error://test-service/user-not-found");
        error.ToString().Should().Be("app-error://test-service/user-not-found");
    }

    [Fact]
    public void Create_ShouldBeExpected()
    {
        // Arrange
        var rootGroup = ErrorGroupUri.Create("scheme", "app");
        var errorGroup = ErrorGroupUri.Create("scheme", "app", "group");

        // Assert
        ErrorUri.Create(rootGroup, "error-code").ToString()
            .Should().Be("scheme://app/error-code");

        ErrorUri.Create(errorGroup, "error-code").ToString()
            .Should().Be("scheme://app/group/error-code");

        ErrorUri.Create(rootGroup, "👋").ToString()
            .Should().Be("scheme://app/👋");

        ErrorUri.Create(rootGroup, "-").ToString()
            .Should().Be("scheme://app/-");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("a/")]
    [InlineData("a/b")]
    [InlineData("/")]
    public void Create_Bad_ShouldThrow(string? errorCode)
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("scheme", "app", "group");

        // Assert
        if (errorCode == null)
            Assert.Throws<ArgumentNullException>(() => ErrorUri.Create(errorGroup, errorCode!));
        else
            Assert.Throws<ArgumentException>(() => ErrorUri.Create(errorGroup, errorCode));
    }

    [Fact]
    public void Parse_ShouldBeExpected()
    {
        // Assert
        ErrorUri.Parse("scheme://app/error-code")
            .Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app"), "error-code"));

        ErrorUri.Parse("scheme://app/group/error-code")
            .Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app", "group"), "error-code"));

        ErrorUri.Parse("scheme://app/groupA/groupB/error-code")
            .Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app", "groupA/groupB"), "error-code"));

        ErrorUri.Parse("scheme://app/👋")
            .Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app"), "👋"));
    }

    [Fact]
    public void TryParse_ShouldBeExpected()
    {
        // Assert
        ErrorUri.TryParse("scheme://app/error-code", out var result)
            .Should().BeTrue();
        result.Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app"), "error-code"));

        ErrorUri.TryParse("scheme://app/group/error-code", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app", "group"), "error-code"));

        ErrorUri.TryParse("scheme://app/groupA/groupB/error-code", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app", "groupA/groupB"), "error-code"));

        ErrorUri.TryParse("scheme://app/👋", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorUri.Create(ErrorGroupUri.Create("scheme", "app"), "👋"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("service")]
    [InlineData("1://service")]
    [InlineData("ht~tp://service/")]
    [InlineData("app-error://service/")]
    [InlineData("app-error://👋/")]
    [InlineData("app-error://service/group/")]
    public void Parse_Bad_ShouldThrow(string? errorUri)
    {
        // Assert
        if (errorUri == null)
        {
            Assert.Throws<ArgumentNullException>(() => ErrorUri.Parse(errorUri!));
            Assert.Throws<ArgumentNullException>(() => ErrorUri.Parse((Uri)null!));
        }
        else if (Uri.TryCreate(errorUri, UriKind.RelativeOrAbsolute, out var uri))
        {
            Assert.Throws<ArgumentException>(() => ErrorUri.Parse(errorUri));
            Assert.Throws<ArgumentException>(() => ErrorUri.Parse(uri));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => ErrorUri.Parse(errorUri));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("service")]
    [InlineData("1://service")]
    [InlineData("ht~tp://service/")]
    [InlineData("app-error://service/")]
    [InlineData("app-error://👋/")]
    [InlineData("app-error://service/group/")]
    public void TryParse_Bad_ShouldBeFalse(string? errorUri)
    {
        // Assert
        ErrorUri.TryParse(errorUri, out var result).Should().BeFalse();
        result.Should().BeNull();

        if (errorUri == null)
        {
            ErrorUri.TryParse((Uri)null!, out result).Should().BeFalse();
            result.Should().BeNull();
        }
        else if (Uri.TryCreate(errorUri, UriKind.RelativeOrAbsolute, out var uri))
        {
            ErrorUri.TryParse(uri, out result).Should().BeFalse();
            result.Should().BeNull();
        }
    }
}