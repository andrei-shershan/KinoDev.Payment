using KinoDev.Payment.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KinoDev.Payment.Infrastructure.Extensions
{
    public static class InfrastructureExtensions
    {
        public static IServiceCollection InitializeInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddScoped<IMongoDbService, MongoDbService>();            
            services.AddScoped<IStripeService, StripeService>();

            return services;
        }
    }
}