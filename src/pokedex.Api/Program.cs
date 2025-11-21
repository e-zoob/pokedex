using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using pokedex.Api.Domain.Services;
using pokedex.Api.Endpoints;
using pokedex.Api.Infrastructure.Clients;
using pokedex.Api.Options;
using pokedex.Api.Validation;
using Serilog;
using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// configs
builder.Services.Configure<PokemonApiOptions>(
    builder.Configuration.GetSection("PokemonApiOptions")
);

// infra
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IPokemonInfoClient, PokemonInfoClient>();

// business
builder.Services.AddScoped<IPokemonApiService, PokemonApiService>();
builder.Services.AddScoped<IPokemonNameValidator, PokemonNameValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Pokedex Api",
        Version = "v1",
        Description = "Minimal Api to get Pokemon information"
    });
});

builder.Services.AddProblemDetails();

try
{
    Log.Information("Starting Pokedex Api");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        if (exception != null)
            logger.LogError(exception, "Unhandled exception occurred");

        var problem = new ProblemDetails
        {
            Title = "An unexpected error occurred",
            Detail = exception?.Message,
            Status = StatusCodes.Status500InternalServerError
        };
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problem.Status ?? 500;
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.MapPokemonEndpoints();

app.Run();

}
catch( Exception ex)
{
    Log.Fatal(ex, "Pokedex Api failed to start");
}
finally
{
    Log.CloseAndFlush();
}
