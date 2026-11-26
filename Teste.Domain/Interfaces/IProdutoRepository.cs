using Teste.Domain.Entities;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<Produto?> ObterPorIdAsync(Guid id);
    Task<List<Produto>> ListarAsync();
    Task<bool> ExisteSkuAsync(Sku sku);
    Task AdicionarAsync(Produto produto);
}
