namespace SocialMedia.Domain.Primitives;

public sealed record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Gone(string code, string description) =>
        new(code, description, ErrorType.Gone);

    public static Error PreconditionFailed(string code, string description) =>
        new(code, description, ErrorType.PreconditionFailed);

    public static Error Unprocessable(string code, string description) =>
        new(code, description, ErrorType.Unprocessable);

    public static Error TooManyRequests(string code, string description) =>
        new(code, description, ErrorType.TooManyRequests);

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error Unavailable(string code, string description) =>
        new(code, description, ErrorType.Unavailable);
}
