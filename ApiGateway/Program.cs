using IdentityServer4.Services;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

var authProviderKey = "IdentityApiKey";


builder.Services.AddAuthentication().
    AddJwtBearer(authProviderKey, x =>
    {
        x.Authority = "https://localhost:7040";
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
}); ;

builder.Services.AddOcelot();

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(ep => ep.MapControllers());

app.UseOcelot().Wait();

app.Run();
