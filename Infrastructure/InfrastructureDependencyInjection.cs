using Application.Interfaces.V1;
using Infrastructure.Persistence;
using Infrastructure.Services.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

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

        services.AddScoped<IOrderGatewayService, OrderGatewayService>();

        return services;
    }
}
