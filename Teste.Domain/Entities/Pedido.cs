using Teste.Domain.Common;
using Teste.Domain.Enums;
using Teste.Domain.Exceptions;
using Teste.Domain.Guards;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Entities;

/// <summary>
/// Agregado Pedido: representa um pedido sendo expedido pelo Centro de Distribuição.
/// Todas as regras do ciclo de vida do pedido ficam aqui dentro, e não no Service ou no Controller.
/// </summary>
public class Pedido : AggregateRoot
{
    // Lista privada: de fora não dá para fazer pedido.Itens.Add(...).
    // Para mexer nos itens é obrigatório usar AdicionarItem e RemoverItem.
    private readonly List<ItemPedido> _itens = new List<ItemPedido>();

    public string Cliente { get; private set; } = string.Empty;
    public Endereco EnderecoEntrega { get; private set; } = null!;
    public ModalidadeFrete ModalidadeFrete { get; private set; }
    public StatusPedido Status { get; private set; }
    public Frete? Frete { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? DespachadoEm { get; private set; }

    public IReadOnlyCollection<ItemPedido> Itens
    {
        get { return _itens.AsReadOnly(); }
    }

    public decimal PesoTotalKg
    {
        get { return _itens.Sum(i => i.PesoTotalKg); }
    }

    // Construtor vazio usado apenas pelo Entity Framework
    private Pedido()
    {
    }

    public Pedido(string cliente, Endereco enderecoEntrega, ModalidadeFrete modalidadeFrete)
    {
        Guard.AgainstNullOrEmpty(cliente, "O cliente é obrigatório.");
        Guard.AgainstMaxLength(cliente, 150, "O cliente deve ter no máximo 150 caracteres.");
        Guard.AgainstNull(enderecoEntrega, "O endereço de entrega é obrigatório.");
        Guard.Against(!Enum.IsDefined(modalidadeFrete), "Modalidade de frete inválida.");

        Cliente = cliente.Trim();
        EnderecoEntrega = enderecoEntrega;
        ModalidadeFrete = modalidadeFrete;
        Status = StatusPedido.Criado;
        CriadoEm = DateTime.UtcNow;
    }

    public void AdicionarItem(Produto produto, int quantidade)
    {
        Guard.AgainstNull(produto, "O produto é obrigatório.");
        Guard.AgainstZeroOrNegative(quantidade, "A quantidade deve ser maior que zero.");
        GarantirStatus(StatusPedido.Criado, "adicionar itens");

        // Se o produto já está no pedido, só aumenta a quantidade
        var itemExistente = _itens.FirstOrDefault(i => i.ProdutoId == produto.Id);
        if (itemExistente != null)
        {
            itemExistente.AumentarQuantidade(quantidade);
            return;
        }

        var novoItem = new ItemPedido(produto.Id, produto.Sku, produto.Nome, produto.PesoKg, quantidade);
        _itens.Add(novoItem);
    }

    /// <summary>Remove o item e o devolve, para o Service poder devolver o estoque.</summary>
    public ItemPedido RemoverItem(Guid produtoId)
    {
        GarantirStatus(StatusPedido.Criado, "remover itens");

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item == null)
            throw new DomainException("O produto informado não faz parte deste pedido.");

        _itens.Remove(item);
        return item;
    }

    public void IniciarSeparacao()
    {
        GarantirStatus(StatusPedido.Criado, "iniciar a separação");
        Guard.Against(_itens.Count == 0, "Não é possível iniciar a separação de um pedido sem itens.");

        Status = StatusPedido.EmSeparacao;
    }

    public void ConcluirSeparacao()
    {
        GarantirStatus(StatusPedido.EmSeparacao, "concluir a separação");

        Status = StatusPedido.Separado;
    }

    public void Despachar(Frete frete)
    {
        Guard.AgainstNull(frete, "O frete é obrigatório.");
        GarantirStatus(StatusPedido.Separado, "despachar");
        Guard.Against(frete.Modalidade != ModalidadeFrete,
            "O frete informado não corresponde à modalidade escolhida no pedido.");

        Frete = frete;
        Status = StatusPedido.Despachado;
        DespachadoEm = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        Guard.Against(Status == StatusPedido.Despachado, "Pedido já despachado não pode ser cancelado.");
        Guard.Against(Status == StatusPedido.Cancelado, "O pedido já está cancelado.");

        Status = StatusPedido.Cancelado;
    }

    private void GarantirStatus(StatusPedido statusEsperado, string acao)
    {
        if (Status != statusEsperado)
            throw new DomainException(
                $"Não é possível {acao}: o pedido está '{Status}' e precisa estar '{statusEsperado}'.");
    }
}
