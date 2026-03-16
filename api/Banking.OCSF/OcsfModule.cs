using Banking.OCSF.Consumer;
using Banking.OCSF.Database;
using Banking.OCSF.Interfaces;
using Banking.OCSF.Messaging;
using Banking.OCSF.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Banking.OCSF;

public static class OcsfModule
{
    public static IServiceCollection AddOcsfModule(
        this IServiceCollection services,
        string pgConnection,
        string rabbitHost,
        string rabbitUser,
        string rabbitPassword
    )
    {
        SetupDatabase(services, pgConnection);
        SetupMessaging(services, rabbitHost, rabbitUser, rabbitPassword);

        return services;
    }

    private static void SetupDatabase(IServiceCollection services, string pgConnection)
    {
        services.AddDbContext<OcsfDbContext>(options =>
            options.UseNpgsql(
                pgConnection,
                pgOptions => pgOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            )
        );
    }

    private static void SetupMessaging(
        IServiceCollection services,
        string host,
        string user,
        string password
    )
    {
        // Single shared connection for the lifetime of the application.
        services.AddSingleton(_ => new RabbitMqConnection(
            new ConnectionFactory
            {
                HostName = host,
                UserName = user,
                Password = password,
                // Automatic connection recovery — reconnects after network blips.
                AutomaticRecoveryEnabled = true,
                // Re-declare queues and consumers after recovery.
                TopologyRecoveryEnabled = true,
            }
        ));

        // The publisher — what the rest of the banking app calls.
        services.AddScoped<IOcsfAuditLogger, RabbitMqAuditLogger>();

        // The consumer — runs as a hosted background service for the app lifetime.
        services.AddHostedService<AuthenticationEventConsumer>();
    }
}
