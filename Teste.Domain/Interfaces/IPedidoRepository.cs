using Teste.Domain.Entities;
using Teste.Domain.Enums;

namespace Teste.Domain.Interfaces;

/// <summary>
/// A interface do repositório fica no Domain, e a implementação (com Entity Framework)
/// fica na Infrastructure. Assim o domínio não depende do banco de dados.
/// </summary>
public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task<List<Pedido>> ListarAsync(StatusPedido? status);
    Task AdicionarAsync(Pedido pedido);
}
