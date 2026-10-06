using MediatR;
using SocialMedia.Application.Abstractions;
using SocialMedia.Contract.Users;

namespace SocialMedia.Application.Users.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(ICurrentUser currentUser)
    : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    public Task<CurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CurrentUserResponse(currentUser.Id));
    }
}
