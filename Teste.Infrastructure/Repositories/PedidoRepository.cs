using Microsoft.EntityFrameworkCore;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Interfaces;
using Teste.Infrastructure.Data;

namespace Teste.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
    {
        // Include: carrega os itens junto com o pedido
        return await _context.Pedidos
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Pedido>> ListarAsync(StatusPedido? status)
    {
        var query = _context.Pedidos
            .AsNoTracking() // somente leitura, mais rápido
            .Include(p => p.Itens)
            .AsQueryable();

        if (status != null)
            query = query.Where(p => p.Status == status);

        return await query
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
    }
}
