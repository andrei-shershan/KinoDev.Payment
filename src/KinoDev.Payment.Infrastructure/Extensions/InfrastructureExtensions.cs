using KinoDev.Payment.Infrastructure.Abstractions;
using KinoDev.Payment.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace KinoDev.Payment.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services)
        {
            // Let's pretend we implement Strategy pattern
            // We can have different implementations in future
            // We can easily switch between different implementations
            // by changing the implementation here
            services.AddScoped<IDbService, MongoDbService>();            
            services.AddScoped<IPaymentProviderService, StripeService>();

            services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}