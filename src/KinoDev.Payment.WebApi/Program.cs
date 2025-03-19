using KinoDev.Payment.Infrastructure.Extensions;
using KinoDev.Payment.Infrastructure.Models;
using KinoDev.Payment.WebApi.ConfigurationSettings;
using KinoDev.Payment.WebApi.Extensions;
using Microsoft.IdentityModel.Protocols.Configuration;
using MongoDB.Driver.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitializeInfrastructure();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var authenticationSettings = builder.Configuration.GetSection("Authentication").Get<AuthenticationSettings>();
if (authenticationSettings == null)
{
    throw new InvalidConfigurationException("Unable to obtain AuthenticationSettings from configuration!");
}

builder.Services.Configure<AuthenticationSettings>(builder.Configuration.GetSection("Authentication"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
// TODO: to constants
builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDb"));

builder.Services.SetupAuthentication(authenticationSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// TODO: to settings
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
