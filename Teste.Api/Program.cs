using System.Text.Json.Serialization;
using Scalar.AspNetCore;
using Teste.Api.Middlewares;
using Teste.Application;
using Teste.Infrastructure;
using Teste.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers + enums como texto no JSON ("Expresso" em vez de 2)
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

// Injeção de dependência de cada camada
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // documentação da API em /scalar
}

app.UseHttpsRedirection();

app.MapControllers();

await DbInitializer.InicializarAsync(app.Services);

app.Run();
