using Teste.Domain.Common;
using Teste.Domain.Exceptions;
using Teste.Domain.Guards;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Entities;

/// <summary>
/// Agregado Produto: controla o próprio estoque.
/// Não tem "set" público: o estado só muda pelos métodos com nome de negócio
/// (DarEntradaEstoque, ReservarEstoque, DevolverEstoque).
/// </summary>
public class Produto : AggregateRoot
{
    public Sku Sku { get; private set; } = null!;
    public string Nome { get; private set; } = string.Empty;
    public decimal PesoKg { get; private set; }
    public int QuantidadeEmEstoque { get; private set; }

    // Construtor vazio usado apenas pelo Entity Framework
    private Produto()
    {
    }

    public Produto(Sku sku, string nome, decimal pesoKg, int estoqueInicial)
    {
        Guard.AgainstNull(sku, "O SKU é obrigatório.");
        Guard.AgainstNullOrEmpty(nome, "O nome é obrigatório.");
        Guard.AgainstMaxLength(nome, 150, "O nome deve ter no máximo 150 caracteres.");
        Guard.AgainstZeroOrNegative(pesoKg, "O peso deve ser maior que zero.");
        Guard.AgainstNegative(estoqueInicial, "O estoque inicial não pode ser negativo.");

        Sku = sku;
        Nome = nome.Trim();
        PesoKg = pesoKg;
        QuantidadeEmEstoque = estoqueInicial;
    }

    public void DarEntradaEstoque(int quantidade)
    {
        Guard.AgainstZeroOrNegative(quantidade, "A quantidade deve ser maior que zero.");

        QuantidadeEmEstoque += quantidade;
    }

    public void ReservarEstoque(int quantidade)
    {
        Guard.AgainstZeroOrNegative(quantidade, "A quantidade deve ser maior que zero.");

        if (quantidade > QuantidadeEmEstoque)
            throw new DomainException(
                $"Estoque insuficiente para o SKU {Sku}. Disponível: {QuantidadeEmEstoque}, solicitado: {quantidade}.");

        QuantidadeEmEstoque -= quantidade;
    }

    public void DevolverEstoque(int quantidade)
    {
        Guard.AgainstZeroOrNegative(quantidade, "A quantidade deve ser maior que zero.");

        QuantidadeEmEstoque += quantidade;
    }
}
