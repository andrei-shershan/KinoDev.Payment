using KinoDev.Payment.Infrastructure.Configuration;
using KinoDev.Payment.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.Payment.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IStripeService, StripeService>();
            return services;
        }
    }
}