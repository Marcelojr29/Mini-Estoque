using Teste.Domain.Common;
using Teste.Domain.Guards;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Entities;

/// <summary>
/// Item de um pedido. Faz parte do agregado Pedido.
/// O construtor é "internal": só o Pedido consegue criar itens.
/// Guarda o ProdutoId (e não o objeto Produto), porque Produto é outro agregado.
/// Nome e peso são copiados do produto no momento em que o item é adicionado.
/// </summary>
public class ItemPedido : Entity
{
    public Guid ProdutoId { get; private set; }
    public Sku Sku { get; private set; } = null!;
    public string NomeProduto { get; private set; } = string.Empty;
    public decimal PesoUnitarioKg { get; private set; }
    public int Quantidade { get; private set; }

    public decimal PesoTotalKg
    {
        get { return PesoUnitarioKg * Quantidade; }
    }

    // Construtor vazio usado apenas pelo Entity Framework
    private ItemPedido()
    {
    }

    internal ItemPedido(Guid produtoId, Sku sku, string nomeProduto, decimal pesoUnitarioKg, int quantidade)
    {
        Guard.AgainstZeroOrNegative(quantidade, "A quantidade deve ser maior que zero.");

        ProdutoId = produtoId;
        Sku = sku;
        NomeProduto = nomeProduto;
        PesoUnitarioKg = pesoUnitarioKg;
        Quantidade = quantidade;
    }

    internal void AumentarQuantidade(int quantidade)
    {
        Guard.AgainstZeroOrNegative(quantidade, "A quantidade deve ser maior que zero.");

        Quantidade += quantidade;
    }
}
