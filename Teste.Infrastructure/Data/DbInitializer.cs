using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Teste.Domain.Entities;
using Teste.Domain.ValueObjects;

namespace Teste.Infrastructure.Data;

public static class DbInitializer
{
    /// <summary>
    /// Cria o banco de dados (se ainda não existir) e cadastra alguns produtos de exemplo.
    /// Em um projeto real, o ideal seria usar Migrations.
    /// </summary>
    public static async Task InicializarAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        var jaTemProdutos = await context.Produtos.AnyAsync();
        if (jaTemProdutos)
            return;

        context.Produtos.Add(new Produto(new Sku("CAMA-QUEEN-001"), "Jogo de Cama Queen 4 Peças", 1.8m, 120));
        context.Produtos.Add(new Produto(new Sku("TOALHA-BANHO-01"), "Toalha de Banho Felpuda", 0.6m, 300));
        context.Produtos.Add(new Produto(new Sku("PANELA-PRESS-5L"), "Panela de Pressão 5L", 2.4m, 80));
        context.Produtos.Add(new Produto(new Sku("VENTILADOR-40"), "Ventilador de Mesa 40cm", 3.9m, 45));
        context.Produtos.Add(new Produto(new Sku("EDREDOM-CASAL"), "Edredom Casal Dupla Face", 2.2m, 60));

        await context.SaveChangesAsync();
    }
}
