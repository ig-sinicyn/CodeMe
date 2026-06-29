namespace CodeMe.ServiceErrors.Abstractions.UnitTests;

public class ErrorGroupUriTests
{
    [Fact]
    public void Equals_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service", "users");
        var same = ErrorGroupUri.Create("app-error", "test-service", "users");
        var sameUpperCase = ErrorGroupUri.Create("APP-ERROR", "TEST-SERVICE", "USERS");
        var otherScheme = ErrorGroupUri.Create("app-error-2", "test-service", "users");
        var otherGroup = ErrorGroupUri.Create("app-error", "test-service-2", "users");
        var otherCategory = ErrorGroupUri.Create("app-error", "test-service", "users-2");

        // Assert
        group.Equals(group).Should().BeTrue();
        group.Equals(same).Should().BeTrue();
        group.Equals(sameUpperCase).Should().BeTrue();
        group.Equals(otherScheme).Should().BeFalse();
        group.Equals(otherGroup).Should().BeFalse();
        group.Equals(otherCategory).Should().BeFalse();
        group.Equals(null).Should().BeFalse();

        group!.Equals((object)group).Should().BeTrue();
        group.Equals((object)same).Should().BeTrue();
        group.Equals((object)sameUpperCase).Should().BeTrue();
        group.Equals((object)otherScheme).Should().BeFalse();
        group.Equals((object)otherGroup).Should().BeFalse();
        group.Equals((object)otherCategory).Should().BeFalse();
        group.Equals((object?)null).Should().BeFalse();

        (group == same).Should().BeTrue();
        (group == otherGroup).Should().BeFalse();
        (group == otherCategory).Should().BeFalse();

        (group != same).Should().BeFalse();
        (group != otherGroup).Should().BeTrue();
        (group != otherCategory).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service", "users");
        var same = ErrorGroupUri.Create("app-error", "test-service", "users");
        var sameUpperCase = ErrorGroupUri.Create("app-error", "TEST-SERVICE", "USERS");
        var otherScheme = ErrorGroupUri.Create("app-error-2", "test-service", "users");
        var otherGroup = ErrorGroupUri.Create("app-error", "test-service-2", "users");
        var otherCategory = ErrorGroupUri.Create("app-error", "test-service", "users-2");

        // Assert
        group.GetHashCode().Should().Be(same.GetHashCode());
        group.GetHashCode().Should().Be(sameUpperCase.GetHashCode());
        group.GetHashCode().Should().NotBe(otherScheme.GetHashCode());
        group.GetHashCode().Should().NotBe(otherGroup.GetHashCode());
        group.GetHashCode().Should().NotBe(otherCategory.GetHashCode());
    }

    [Fact]
    public void RootGroup_Properties_ShouldBeExpected()
    {
        // Arrange
        var rootGroup = ErrorGroupUri.Create("app-error", "test-service");

        // Assert
        rootGroup.Scheme.Should().Be("app-error");
        rootGroup.Application.Should().Be("test-service");
        rootGroup.Category.Should().BeNull();
        rootGroup.Uri.ToString().Should().Be("app-error://test-service/");
        rootGroup.ToString().Should().Be("app-error://test-service/");
        rootGroup.IsApplicationGroup.Should().BeTrue();
        rootGroup.ApplicationGroup.Should().Be(rootGroup);
        rootGroup.ParentGroup.Should().BeNull();
    }

    [Fact]
    public void ErrorGroup_Properties_ShouldBeExpected()
    {
        // Arrange
        var errorGroup = ErrorGroupUri.Create("app-error", "test-service", "users");

        // Assert
        errorGroup.Scheme.Should().Be("app-error");
        errorGroup.Application.Should().Be("test-service");
        errorGroup.Category.Should().Be("users");
        errorGroup.Uri.ToString().Should().Be("app-error://test-service/users/");
        errorGroup.ToString().Should().Be("app-error://test-service/users/");
        errorGroup.IsApplicationGroup.Should().BeFalse();
        errorGroup.ApplicationGroup.Should().Be(
            ErrorGroupUri.Create("app-error", "test-service"));
        errorGroup.ParentGroup.Should().Be(
            ErrorGroupUri.Create("app-error", "test-service"));
    }

    [Fact]
    public void ChildGroup_Properties_ShouldBeExpected()
    {
        // Arrange
        var childGroup = ErrorGroupUri.Create("app-error", "test-service", "users", "registration");

        // Assert
        childGroup.Scheme.Should().Be("app-error");
        childGroup.Application.Should().Be("test-service");
        childGroup.Category.Should().Be("users/registration");
        childGroup.Uri.ToString().Should().Be("app-error://test-service/users/registration/");
        childGroup.ToString().Should().Be("app-error://test-service/users/registration/");
        childGroup.IsApplicationGroup.Should().BeFalse();
        childGroup.ApplicationGroup.Should().Be(
            ErrorGroupUri.Create("app-error", "test-service"));
        childGroup.ParentGroup.Should().Be(
            ErrorGroupUri.Create("app-error", "test-service", "users"));
    }

    [Fact]
    public void Create_ShouldBeExpected()
    {
        // Assert
        ErrorGroupUri.Create("scheme", "app").ToString()
            .Should().Be("scheme://app/");

        ErrorGroupUri.Create("scheme", "app", "group").ToString()
            .Should().Be("scheme://app/group/");

        ErrorGroupUri.Create("scheme", "app", "groupA/groupB").ToString()
            .Should().Be("scheme://app/groupA/groupB/");

        ErrorGroupUri.Create("scheme", "app", "groupA/groupB/").ToString()
            .Should().Be("scheme://app/groupA/groupB/");

        ErrorGroupUri.Create("scheme", "app", new[] { "/groupA/", "/groupB/" }.AsEnumerable()).ToString()
            .Should().Be("scheme://app/groupA/groupB/");

        ErrorGroupUri.Create("scheme", "app", "////groupA////groupB////").ToString()
            .Should().Be("scheme://app/groupA/groupB/");

        ErrorGroupUri.Create("scheme", "app", "///groupA/", "groupB///groupC").ToString()
            .Should().Be("scheme://app/groupA/groupB/groupC/");

        ErrorGroupUri.Create("scheme", "app", "/groupA/", "", "groupB").ToString()
            .Should().Be("scheme://app/groupA/groupB/");

        ErrorGroupUri.Create("scheme", "app", "👋").ToString()
            .Should().Be("scheme://app/👋/");

        ErrorGroupUri.Create("scheme", "app", "-").ToString()
            .Should().Be("scheme://app/-/");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("👋")]
    [InlineData("a/a")]
    public void Create_BadScheme_ShouldThrow(string? scheme)
    {
        // Assert
        if (scheme == null)
            Assert.Throws<ArgumentNullException>(() => ErrorGroupUri.Create(scheme!, "app"));
        else
            Assert.Throws<ArgumentException>(() => ErrorGroupUri.Create(scheme, "app"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("👋")]
    [InlineData("a/a")]
    public void Create_BadApplication_ShouldThrow(string? application)
    {
        // Assert
        if (application == null)
            Assert.Throws<ArgumentNullException>(() => ErrorGroupUri.Create("scheme", application!));
        else
            Assert.Throws<ArgumentException>(() => ErrorGroupUri.Create("scheme", application));
    }

    [Fact]
    public void Parse_ShouldBeExpected()
    {
        // Assert
        ErrorGroupUri.Parse("scheme://app")
            .Should().Be(ErrorGroupUri.Create("scheme", "app"));

        ErrorGroupUri.Parse("scheme://app/")
            .Should().Be(ErrorGroupUri.Create("scheme", "app"));

        ErrorGroupUri.Parse("scheme://app/group/")
            .Should().Be(ErrorGroupUri.Create("scheme", "app", "group"));

        ErrorGroupUri.Parse("scheme://app/groupA/groupB/")
            .Should().Be(ErrorGroupUri.Create("scheme", "app", "groupA/groupB"));

        ErrorGroupUri.Parse("scheme://app////groupA////groupB///")
            .Should().Be(ErrorGroupUri.Create("scheme", "app", "groupA/groupB"));

        ErrorGroupUri.Parse("scheme://app/👋/")
            .Should().Be(ErrorGroupUri.Create("scheme", "app", "👋"));
    }

    [Fact]
    public void TryParse_ShouldBeExpected()
    {
        // Assert
        ErrorGroupUri.TryParse("scheme://app", out var result)
            .Should().BeTrue();
        result.Should().Be(ErrorGroupUri.Create("scheme", "app"));

        ErrorGroupUri.TryParse("scheme://app/", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorGroupUri.Create("scheme", "app"));

        ErrorGroupUri.TryParse("scheme://app/group/", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorGroupUri.Create("scheme", "app", "group"));

        ErrorGroupUri.TryParse("scheme://app/groupA/groupB/", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorGroupUri.Create("scheme", "app", "groupA/groupB"));

        ErrorGroupUri.TryParse("scheme://app////groupA////groupB///", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorGroupUri.Create("scheme", "app", "groupA/groupB"));

        ErrorGroupUri.TryParse("scheme://app/👋/", out result)
            .Should().BeTrue();
        result.Should().Be(ErrorGroupUri.Create("scheme", "app", "👋"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("service")]
    [InlineData("1://service")]
    [InlineData("ht~tp://service/")]
    [InlineData("app-error://👋/")]
    [InlineData("app-error://service/error")]
    public void Parse_Bad_ShouldThrow(string? errorUri)
    {
        // Assert
        if (errorUri == null)
        {
            Assert.Throws<ArgumentNullException>(() => ErrorGroupUri.Parse(errorUri!));
            Assert.Throws<ArgumentNullException>(() => ErrorGroupUri.Parse((Uri)null!));
        }
        else if (Uri.TryCreate(errorUri, UriKind.RelativeOrAbsolute, out var uri))
        {
            Assert.Throws<ArgumentException>(() => ErrorGroupUri.Parse(errorUri));
            Assert.Throws<ArgumentException>(() => ErrorGroupUri.Parse(uri));
        }
        else
        {
            Assert.Throws<ArgumentException>(() => ErrorGroupUri.Parse(errorUri));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("service")]
    [InlineData("1://service")]
    [InlineData("ht~tp://service/")]
    [InlineData("app-error://👋/")]
    [InlineData("app-error://service/error")]
    public void TryParse_Bad_ShouldBeFalse(string? errorUri)
    {
        // Assert
        ErrorGroupUri.TryParse(errorUri, out var result).Should().BeFalse();
        result.Should().BeNull();

        if (errorUri == null)
        {
            ErrorGroupUri.TryParse((Uri)null!, out result).Should().BeFalse();
            result.Should().BeNull();
        }
        else if (Uri.TryCreate(errorUri, UriKind.RelativeOrAbsolute, out var uri))
        {
            ErrorGroupUri.TryParse(uri, out result).Should().BeFalse();
            result.Should().BeNull();
        }
    }


    [Fact]
    public void Contains_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service", "users");
        var same = ErrorGroupUri.Create("app-error", "test-service", "users");
        var sameUpperCase = ErrorGroupUri.Create("APP-ERROR", "TEST-SERVICE", "USERS");
        var child = ErrorGroupUri.Create("app-error", "test-service", "users", "registration");
        var otherScheme = ErrorGroupUri.Create("app-error-2", "test-service", "users");
        var otherGroup = ErrorGroupUri.Create("app-error", "test-service-2", "users");
        var otherCategory = ErrorGroupUri.Create("app-error", "test-service", "users-2");

        // Assert
        group.Contains(group).Should().BeTrue();
        group.Contains(same).Should().BeTrue();
        group.Contains(sameUpperCase).Should().BeTrue();
        group.Contains(child).Should().BeTrue();
        group.Contains(otherScheme).Should().BeFalse();
        group.Contains(otherGroup).Should().BeFalse();
        group.Contains(otherCategory).Should().BeFalse();

        group.ApplicationGroup.Contains(group).Should().BeTrue();
        group.Contains(group.ApplicationGroup).Should().BeFalse();
    }


    [Fact]
    public void SubGroup_ShouldBeExpected()
    {
        // Arrange
        var group = ErrorGroupUri.Create("app-error", "test-service", "users");
        var child = ErrorGroupUri.Create("app-error", "test-service", "users", "registration");

        // Assert
        group.SubGroup("registration").Should().Be(child);
        group.ApplicationGroup.SubGroup("users").Should().Be(group);
        group.ApplicationGroup.SubGroup("users/registration").Should().Be(child);
        group.ApplicationGroup.SubGroup("users", "registration").Should().Be(child);
    }
}