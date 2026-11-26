using Microsoft.EntityFrameworkCore;
using Teste.Domain.Entities;
using Teste.Domain.Interfaces;
using Teste.Infrastructure.Data.Configurations;

namespace Teste.Infrastructure.Data;

/// <summary>
/// DbContext do Entity Framework. Ele também implementa o IUnitOfWork,
/// porque o SaveChanges já salva todas as alterações em uma única transação.
/// </summary>
public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
        modelBuilder.ApplyConfiguration(new PedidoConfiguration());
        modelBuilder.ApplyConfiguration(new ItemPedidoConfiguration());
    }

    public async Task<int> CommitAsync()
    {
        return await SaveChangesAsync();
    }
}
