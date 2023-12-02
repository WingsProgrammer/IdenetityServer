using MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;
using MS.Services.Identity.Identity;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Abstractions.CQRS.Query;

namespace MS.Services.Identity.Api.Identity
{
    public static class ForgotePasswordSendCodeEndpoint
    {
        public static IEndpointRouteBuilder MapForgotePasswordSendCodeEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost($"{IdentityConfigs.IdentityPrefixUri}/ForgetPasswordSendCodeToMobile", SendCode)
                .WithTags(IdentityConfigs.Tag)
                //.Produces(StatusCodes.Status401Unauthorized)
                .WithDisplayName("Send Code For Mobile");

            return endpoints;
        }

        private static async Task<IResult> SendCode(
            SendCodeForMobileRequest request, ICommandProcessor queryProcessor, CancellationToken cancellationToken)
        {
            var command = new SendCodeForMobileCommand(request.MobileNumber);

            var commandResult=await queryProcessor.SendAsync(command, cancellationToken);
            
            if (commandResult.Errors.Count>0)
            {
                return Results.BadRequest(commandResult.Errors[0].Message ?? commandResult.Errors[0].Reasons[0].Message);
            }

            return Results.Ok(commandResult);
        }
    }
}
