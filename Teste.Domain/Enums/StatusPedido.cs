namespace Teste.Domain.Enums;

/// <summary>
/// Ciclo de vida de um pedido dentro do Centro de Distribuição.
/// Criado -> EmSeparacao -> Separado -> Despachado
/// (Cancelado é possível em qualquer etapa antes de Despachado)
/// </summary>
public enum StatusPedido
{
    Criado = 1,
    EmSeparacao = 2,
    Separado = 3,
    Despachado = 4,
    Cancelado = 5
}
