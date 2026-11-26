using Microsoft.Extensions.DependencyInjection;
using Teste.Application.Interfaces;
using Teste.Application.Services;
using Teste.Domain.Interfaces;
using Teste.Domain.Services;
using Teste.Domain.Strategies;

namespace Teste.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IPedidoService, PedidoService>();

        // Estratégias de frete: para criar uma nova modalidade,
        // basta criar a classe e adicionar uma linha aqui.
        services.AddScoped<IFreteStrategy, FreteEconomicoStrategy>();
        services.AddScoped<IFreteStrategy, FreteExpressoStrategy>();
        services.AddScoped<IFreteStrategy, RetiradaNaLojaStrategy>();
        services.AddScoped<ICalculadoraFrete, CalculadoraFrete>();

        return services;
    }
}
