using Application.Interfaces.V1;
using Contracts.Grpc.Wallet;
using Infrastructure.BackgroundJobs;
using Infrastructure.Grpc;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Services.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProtoBuf.Grpc.ClientFactory;

namespace Infrastructure;

/// <summary>
/// Registers Infrastructure services that connect the Gateway to persistence, RabbitMQ publishing,
/// background processing, and OrderService gRPC communication.
/// </summary>
public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<GatewayDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        services.Configure<OrderServiceGrpcOptions>(configuration.GetSection("OrderService"));
        services.AddCodeFirstGrpcClient<IWalletGrpcService>(options =>
        {
            var grpcAddress = configuration.GetValue<string>("OrderService:GrpcAddress")
                ?? throw new InvalidOperationException("OrderService gRPC address was not found.");

            options.Address = new Uri(grpcAddress);
        });

        services.AddScoped<IOrderGatewayService, OrderGatewayService>();
        services.AddScoped<IWalletGatewayService, WalletGatewayService>();
        services.AddScoped<IOutboxPublisherService, OutboxPublisherService>();
        services.AddHostedService<OutboxPublisherBackgroundService>();

        return services;
    }
}
