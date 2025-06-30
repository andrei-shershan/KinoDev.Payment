using KinoDev.Payment.Infrastructure.Extensions;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Payment.WebApi.ConfigurationSettings;
using KinoDev.Payment.WebApi.Extensions;
using Microsoft.IdentityModel.Protocols.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add Application Insights telemetry collection
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.InitializeInfrastructure();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
if (authenticationSettings == null)
{
    throw new InvalidConfigurationException("Unable to obtain AuthenticationSettings from configuration!");
}

// TODO: to constants
builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDb"));

builder.Services.SetupAuthentication(authenticationSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var disableHttpsRedirection = builder.Configuration.GetValue<bool>("DisableHttpsRedirection");
if (!disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

