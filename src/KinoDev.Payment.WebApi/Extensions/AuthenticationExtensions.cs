using KinoDev.Payment.WebApi.ConfigurationSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace KinoDev.Payment.WebApi.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection SetupAuthentication(
            this IServiceCollection services,
            AuthenticationSettings authenticationSettings
            )
        {
            System.Console.WriteLine("Setting up authentication" + authenticationSettings.Secret + authenticationSettings.Issuer + authenticationSettings.Audiences.Internal);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = authenticationSettings.Issuer,
                        ValidAudience = authenticationSettings.Audiences.Internal,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            return services;
        }
    }
}