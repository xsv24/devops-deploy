using FluentAssertions;

namespace DevopsDeploy.Test;

public class UtilsTests
{
    [Theory]
    [InlineData("cleaned")]
    [InlineData("CLEANED")]
    [InlineData(" cleaned ")]
    [InlineData(" CLEANED ")]
    public void SanitiseTrimsAndConvertsToLower(string value)
    {
        value.Sanitise().Should().Be("cleaned");
    }

    [Theory]
    [InlineData("cleaned")]
    [InlineData("CLEANED")]
    [InlineData(" cleaned ")]
    [InlineData(" CLEANED ")]
    public void GetOrDefaultSanitisesKey(string value)
    {
        var collection = new Dictionary<string, string>
        {
            { "CLEANED", "invalid" },
            { " cleaned ", "invalid" },
            { "cleaned", "actually clean" }
        };

        collection.GetOrDefault(value).Should().Be("actually clean");
        collection.GetOrDefault("junk").Should().BeNull();
    }
}