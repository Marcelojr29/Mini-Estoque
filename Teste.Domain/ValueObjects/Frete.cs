using Teste.Domain.Enums;
using Teste.Domain.Guards;

namespace Teste.Domain.ValueObjects;

/// <summary>Value Object: resultado do cálculo de frete (valor + prazo).</summary>
public record Frete
{
    public ModalidadeFrete Modalidade { get; private set; }
    public decimal Valor { get; private set; }
    public int PrazoEmDias { get; private set; }

    // Construtor vazio usado apenas pelo Entity Framework
    private Frete()
    {
    }

    public Frete(ModalidadeFrete modalidade, decimal valor, int prazoEmDias)
    {
        Guard.AgainstNegative(valor, "O valor do frete não pode ser negativo.");
        Guard.AgainstNegative(prazoEmDias, "O prazo de entrega não pode ser negativo.");

        Modalidade = modalidade;
        Valor = Math.Round(valor, 2);
        PrazoEmDias = prazoEmDias;
    }
}
