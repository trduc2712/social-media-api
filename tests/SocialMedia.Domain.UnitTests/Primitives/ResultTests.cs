using SocialMedia.Domain.Primitives;

namespace SocialMedia.Domain.UnitTests.Primitives;

public class ResultTests
{
    private static readonly Error SampleError = Error.Conflict("Sample.Conflict", "A sample conflict.");

    [Fact]
    public void Success_ShouldBeSuccessfulWithoutError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(Error.None, result.Error);
    }

    [Fact]
    public void Failure_ShouldCarryError()
    {
        var result = Result.Failure(SampleError);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Equal(SampleError, result.Error);
    }

    [Fact]
    public void Failure_ShouldThrow_WhenErrorIsNone()
    {
        Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));
    }

    [Fact]
    public void SuccessOfT_ShouldExposeValue()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Value_ShouldThrow_WhenResultFailed()
    {
        var result = Result.Failure<int>(SampleError);

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateSuccess_FromValue()
    {
        Result<string> result = "value";

        Assert.True(result.IsSuccess);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void ImplicitConversion_ShouldCreateFailure_FromError()
    {
        Result<string> result = SampleError;

        Assert.True(result.IsFailure);
        Assert.Equal(SampleError, result.Error);
    }
}
