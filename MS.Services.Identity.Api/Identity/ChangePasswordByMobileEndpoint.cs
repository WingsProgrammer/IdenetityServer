using MS.Services.Identity.Application.Identity.Features.ForgotMoileNumber;
using MS.Services.Identity.Identity;
using MsftFramework.Abstractions.CQRS.Command;

namespace MS.Services.Identity.Api.Identity
{
    public static class ChangePasswordByMobileEndpoint
    {
        public static IEndpointRouteBuilder MapChangePasswordByMobileEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost($"{IdentityConfigs.IdentityPrefixUri}/ChangePasswordByMobile", ChangePassword)
                .WithTags(IdentityConfigs.Tag)
                //.Produces(StatusCodes.Status401Unauthorized)
                .WithDisplayName("Change User Password By Mobile Number");

            return endpoints;
        }

        private static async Task<IResult> ChangePassword(
            ChangePasswordByMobileRequest request, ICommandProcessor queryProcessor, CancellationToken cancellationToken)
        {
            var command = new ChangePasswordByMobileCommand(request.MobileNumber,request.NewPassword,request.ConfirmNewPassword,request.VerifyCode);

           var result= await queryProcessor.SendAsync(command, cancellationToken);

            return Results.Ok(result);
        }
    }
}
