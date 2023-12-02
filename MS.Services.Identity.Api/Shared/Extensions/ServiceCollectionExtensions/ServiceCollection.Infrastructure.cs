using System.Reflection;
using MsftFramework.Caching.InMemory;
using MsftFramework.Core.Caching;
using MsftFramework.Core.Extensions.Configuration;
using MsftFramework.Core.Extensions.DependencyInjection;
using MsftFramework.Core.Persistence.EfCore;
using MsftFramework.CQRS;
using MsftFramework.Email;
using MsftFramework.Logging;
using MsftFramework.Messaging;
using MsftFramework.Messaging.Postgres.Extensions;
using MsftFramework.Messaging.Transport.Rabbitmq;
using MsftFramework.Monitoring;
using MsftFramework.Persistence.EfCore.Postgres;
using MsftFramework.Scheduling.Internal;
using MsftFramework.Scheduling.Internal.Extensions;
using MsftFramework.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using MsftFramework.Abstractions.CQRS.Command;
using MsftFramework.Abstractions.CQRS.Query;
using MS.Services.Identity.Users.Features.GettingUsers;
using MsftFramework.Core.AssemblyHelper;
using MsftFramework.Core.IdsGenerator;
using MsftFramework.Caching.Redis;

namespace MS.Services.Identity.Shared.Extensions.ServiceCollectionExtensions;

public static class ServiceCollection
{
    public static WebApplicationBuilder AddInfrastructure(
        this WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        AddInfrastructure(builder.Services, configuration);

        return builder;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblies = AssemblyExtractor.GetDomainAssemblies("MS");
        services.AddCore(configuration);
        services.AddEmailService(configuration);
        SnowFlakIdGenerator.Configure(1);
        services.AddCustomValidators(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddCqrs(assemblies,doMoreActions: s =>
        {
            s.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>))
                .AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamRequestValidationBehavior<,>))
                .AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamLoggingBehavior<,>))
                .AddScoped(typeof(IStreamPipelineBehavior<,>), typeof(StreamCachingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(InvalidateCachingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(EfTxBehavior<,>));
        });

        services.AddMonitoring(healthChecksBuilder =>
        {
            var postgresOptions = configuration.GetOptions<PostgresOptions>(nameof(PostgresOptions));
            healthChecksBuilder.AddNpgSql(
                postgresOptions.ConnectionString,
                name: "Identity-Postgres-Check",
                tags: new[] { "identity-postgres" });

            var rabbitMqOptions = configuration.GetOptions<RabbitConfiguration>(nameof(RabbitConfiguration));

            healthChecksBuilder.AddRabbitMQ(
                $"amqp://{rabbitMqOptions.UserName}:{rabbitMqOptions.Password}@{rabbitMqOptions.HostName}{rabbitMqOptions.VirtualHost}",
                name: "IdentityService-RabbitMQ-Check",
                tags: new[] { "identity-rabbitmq" });
        });


      
        services.AddPostgresMessaging(configuration);

        services.AddInternalScheduler(configuration);

        services.AddRabbitMqTransport(configuration);

        services.AddCustomRedisCache(configuration);


        return services;
    }
}
