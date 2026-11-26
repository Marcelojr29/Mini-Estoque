using Teste.Domain.Enums;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Strategies;

public class FreteExpressoStrategy : IFreteStrategy
{
    private const decimal TaxaBase = 30.00m;
    private const decimal ValorPorKg = 4.90m;

    public ModalidadeFrete Modalidade => ModalidadeFrete.Expresso;

    public Frete Calcular(decimal pesoTotalKg, Endereco destino)
    {
        var valor = TaxaBase + (pesoTotalKg * ValorPorKg);
        var prazo = destino.EstaNaRegiaoSul() ? 1 : 3;

        return new Frete(Modalidade, valor, prazo);
    }
}
