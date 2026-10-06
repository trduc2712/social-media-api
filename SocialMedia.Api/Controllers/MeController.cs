using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Users.GetCurrentUser;
using SocialMedia.Contract.Users;

namespace SocialMedia.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/me")]
[Tags("Me")]
[Produces("application/json")]
public sealed class MeController(ISender sender) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the current user")]
    [EndpointDescription("Returns the user identified by the bearer token of the request.")]
    [ProducesResponseType<CurrentUserResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CurrentUserResponse>> Get(CancellationToken cancellationToken)
    {
        return await sender.Send(new GetCurrentUserQuery(), cancellationToken);
    }
}
