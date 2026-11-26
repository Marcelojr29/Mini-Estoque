using Microsoft.EntityFrameworkCore;
using Teste.Domain.Entities;
using Teste.Domain.Interfaces;
using Teste.Domain.ValueObjects;
using Teste.Infrastructure.Data;

namespace Teste.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _context.Produtos
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<bool> ExisteSkuAsync(Sku sku)
    {
        return await _context.Produtos.AnyAsync(p => p.Sku == sku);
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
    }
}
