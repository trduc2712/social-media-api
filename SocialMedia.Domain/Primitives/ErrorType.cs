namespace SocialMedia.Domain.Primitives;

public enum ErrorType
{
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    Gone,
    PreconditionFailed,
    Unprocessable,
    TooManyRequests,
    Failure,
    Unavailable,
}
