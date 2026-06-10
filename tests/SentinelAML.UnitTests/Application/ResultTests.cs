using SentinelAML.Application.Common.Models;

namespace SentinelAML.UnitTests.Application;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSucceededResult()
    {
        var result = Result.Success();

        Assert.True(result.Succeeded);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Failure_ReturnsFailedResultWithErrors()
    {
        var result = Result.Failure("An error occurred.");

        Assert.False(result.Succeeded);
        Assert.Single(result.Errors);
        Assert.Equal("An error occurred.", result.Errors[0]);
    }
}
