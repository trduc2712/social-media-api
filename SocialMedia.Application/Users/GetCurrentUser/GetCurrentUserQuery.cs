using MediatR;
using SocialMedia.Contract.Users;

namespace SocialMedia.Application.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery : IRequest<CurrentUserResponse>;
