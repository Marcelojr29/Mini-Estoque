using Teste.Domain.Enums;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Strategies;

public class FreteEconomicoStrategy : IFreteStrategy
{
    private const decimal TaxaBase = 15.00m;
    private const decimal ValorPorKg = 2.50m;

    public ModalidadeFrete Modalidade => ModalidadeFrete.Economico;

    public Frete Calcular(decimal pesoTotalKg, Endereco destino)
    {
        var valor = TaxaBase + (pesoTotalKg * ValorPorKg);
        var prazo = destino.EstaNaRegiaoSul() ? 5 : 10;

        return new Frete(Modalidade, valor, prazo);
    }
}
