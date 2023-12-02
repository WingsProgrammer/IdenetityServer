using System.Reflection;
using MS.Services.Identity;
using MS.Services.Identity.Api.Extensions.ApplicationBuilderExtensions;
using MS.Services.Identity.Api.Extensions.ServiceCollectionExtensions;
using Hellang.Middleware.ProblemDetails;
using MsftFramework.Logging;
using MsftFramework.Security.Jwt;
using MsftFramework.Swagger;
using MsftFramework.Web;
using MsftFramework.Web.Extensions.ApplicationBuilderExtensions;
using MsftFramework.Web.Extensions.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Serilog;
using MS.Services.Identity.Users;
using KavehNegarSmsProvider;
using MsftFramework.Core.IdsGenerator;
using MsftFramework.Caching.Redis;

// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
// https://benfoster.io/blog/mvc-to-minimal-apis-aspnet-6/
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((env, c) =>
{
    // Handling Captive Dependency Problem
    // https://ankitvijay.net/2020/03/17/net-core-and-di-beware-of-captive-dependency/
    // https://levelup.gitconnected.com/top-misconceptions-about-dependency-injection-in-asp-net-core-c6a7afd14eb4
    // https://blog.ploeh.dk/2014/06/02/captive-dependency/
    if (env.HostingEnvironment.IsDevelopment() || env.HostingEnvironment.IsEnvironment("tests") ||
        env.HostingEnvironment.IsStaging())
    {
        c.ValidateScopes = true;
    }
});

builder.Services.AddControllers(options =>
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.AddCompression();

builder.AddCustomProblemDetails();

builder.Host.AddCustomSerilog();

builder.AddCustomSwagger(builder.Configuration, typeof(IdentityRoot).Assembly);

builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCustomJwtAuthentication(builder.Configuration);

builder.Services.AddCustomAuthorization();

builder.AddIdentityModule();

#region Providers

builder.Services.AddKaveNegarSMSProviderService(builder.Configuration);

#endregion
var app = builder.Build();

var environment = app.Environment;

if (environment.IsDevelopment() || environment.IsEnvironment("docker"))
{
    app.UseDeveloperExceptionPage();

    // Minimal Api not supported versioning in .net 6
}
    app.UseCustomSwagger();

app.UseProblemDetails();

app.UseRouting();

app.UseAppCors();
app.UseAccessTokenValidator();
app.UseCustomHealthCheck();

await app.ConfigureIdentityModule(environment, app.Logger);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapIdentityModuleEndpoints();
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

await app.RunAsync();


namespace MS.Services.Identity.Api
{
    public partial class Program
    {
    }
}
