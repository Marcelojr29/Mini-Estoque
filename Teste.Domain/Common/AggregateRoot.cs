namespace Teste.Domain.Common;

/// <summary>
/// Marca a "raiz" de um Agregado.
/// Agregado = um grupo de objetos que é tratado como uma unidade.
/// Exemplo: Pedido é a raiz e ItemPedido faz parte dele. Os itens só podem
/// ser alterados através do Pedido, nunca diretamente.
/// </summary>
public abstract class AggregateRoot : Entity
{
}
