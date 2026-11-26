namespace Teste.Domain.Common;

/// <summary>
/// Classe base para todas as entidades.
/// Uma Entidade tem IDENTIDADE (o Id): dois pedidos com os mesmos dados
/// continuam sendo pedidos diferentes, porque têm Ids diferentes.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
