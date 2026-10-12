using SocialMedia.Domain.Primitives;

namespace SocialMedia.Domain.UnitTests.Primitives;

public class ErrorTests
{
    public static TheoryData<Func<string, string, Error>, ErrorType> Factories => new()
    {
        { Error.Validation, ErrorType.Validation },
        { Error.Unauthorized, ErrorType.Unauthorized },
        { Error.Forbidden, ErrorType.Forbidden },
        { Error.NotFound, ErrorType.NotFound },
        { Error.Conflict, ErrorType.Conflict },
        { Error.Gone, ErrorType.Gone },
        { Error.PreconditionFailed, ErrorType.PreconditionFailed },
        { Error.Unprocessable, ErrorType.Unprocessable },
        { Error.TooManyRequests, ErrorType.TooManyRequests },
        { Error.Failure, ErrorType.Failure },
        { Error.Unavailable, ErrorType.Unavailable },
    };

    [Theory]
    [MemberData(nameof(Factories))]
    public void Factory_ShouldSetCodeDescriptionAndType(Func<string, string, Error> factory, ErrorType type)
    {
        var error = factory("Sample.Code", "A sample description.");

        Assert.Equal("Sample.Code", error.Code);
        Assert.Equal("A sample description.", error.Description);
        Assert.Equal(type, error.Type);
    }

    [Fact]
    public void Factories_ShouldCoverEveryErrorType()
    {
        Assert.Equal(Enum.GetValues<ErrorType>().Length, Factories.Count);
    }
}
