using Ardalis.ApiEndpoints;
using Ardalis.GuardClauses;
using MsftFramework.Abstractions.CQRS.Query;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;
using MS.Services.Identity.Identity;

namespace MS.Services.Identity.Users.Features.GettingUsers;

// https://www.youtube.com/watch?v=SDu0MA6TmuM
// https://github.com/ardalis/ApiEndpoints
public class GetUsersEndpoint : EndpointBaseAsync
    .WithRequest<GetUsersRequest?>
    .WithActionResult<GetUsersResult>
{
    private readonly IQueryProcessor _queryProcessor;

    public GetUsersEndpoint(IQueryProcessor queryProcessor)
    {
        _queryProcessor = queryProcessor;
    }

    [HttpGet(UsersConfigs.UsersPrefixUri, Name = "GetUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Get all users",
        OperationId = "GetUsers",
        Tags = new[] { UsersConfigs.Tag })]
    public override async Task<ActionResult<GetUsersResult>> HandleAsync(
        [FromQuery] GetUsersRequest? request,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(request, nameof(request));

        var result = await _queryProcessor.SendAsync(
            new GetUsers
            {
                Filters = request.Filters,
                Includes = request.Includes,
                Page = request.Page,
                Sorts = request.Sorts,
                PageSize = request.PageSize
            },
            cancellationToken);



        return Ok(result);
    }
}


//public static class GetUsersEndpoint
//{
//    [SwaggerOperation(
//        Summary = "Get all users",
//        Description = "Get all users",
//        OperationId = "GetUsers",
//        Tags = new[] { UsersConfigs.Tag })]
//    public static IEndpointRouteBuilder MapGetUsersEndpoint(this IEndpointRouteBuilder endpoint)
//    {
//        endpoint.MapGet($"{UsersConfigs.UsersPrefixUri}/GetUsers", GetUsers)
//            .WithTags(UsersConfigs.Tag)
//            .RequireAuthorization()
//            .Produces<GetUsersResult>()
//            .Produces(StatusCodes.Status200OK)
//            .Produces(StatusCodes.Status400BadRequest)
//            .Produces(StatusCodes.Status401Unauthorized)
//            .WithName("GetUsers")
//            .WithDisplayName("GetAllUsers");

//        return endpoint;
//    }

//    private static async Task<IResult> GetUsers([FromQuery] GetUsersRequest? request, IQueryProcessor _queryProcessor, CancellationToken cancellationToken)
//    {
//        Guard.Against.Null(request, nameof(request));

//        var result = await _queryProcessor.SendAsync(
//            new GetUsers
//            {
//                Filters = request.Filters,
//                Includes = request.Includes,
//                Page = request.Page,
//                Sorts = request.Sorts,
//                PageSize = request.PageSize
//            },
//            cancellationToken);


//        return Results.Ok(result);
//    }

//}