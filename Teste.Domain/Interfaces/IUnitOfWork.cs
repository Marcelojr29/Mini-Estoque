namespace Teste.Domain.Interfaces;

/// <summary>
/// Unit of Work: salva todas as alterações de uma operação de uma vez só (uma transação).
/// Exemplo: reservar o estoque do produto + adicionar o item no pedido = salva tudo ou nada.
/// </summary>
public interface IUnitOfWork
{
    Task<int> CommitAsync();
}
