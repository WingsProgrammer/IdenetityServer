using MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;
using MS.Services.Identity.Identity;
using MsftFramework.Abstractions.CQRS.Command;

namespace MS.Services.Identity.Api.Identity
{
    public static class VerifyCodeForMobileEndpoint
    {
        public static IEndpointRouteBuilder MapVerifyCodeForMobileEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost($"{IdentityConfigs.IdentityPrefixUri}/VerifyCodeForMobile", SendCode)
                .WithTags(IdentityConfigs.Tag)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithDisplayName("Send Code For Mobile");

            return endpoints;
        }

        private static async Task<IResult> SendCode(
            VerifyCodeForMobileRequest request, ICommandProcessor queryProcessor, CancellationToken cancellationToken)
        {
            var command = new VerifyCodeForMobileCommand(request.Mobile,request.VerifyCode);

            var res=await queryProcessor.SendAsync(command, cancellationToken);
            if (res.Errors.Count>0)
            {
                return Results.NotFound();
            }
            return Results.Ok(res);
        }
    }
}
