using Teste.Domain.Enums;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Strategies;

/// <summary>
/// O cliente retira na loja: não tem custo, mas tem o prazo de transferência do CD até a loja.
/// </summary>
public class RetiradaNaLojaStrategy : IFreteStrategy
{
    public ModalidadeFrete Modalidade => ModalidadeFrete.RetiradaNaLoja;

    public Frete Calcular(decimal pesoTotalKg, Endereco destino)
    {
        var prazo = destino.EstaNaRegiaoSul() ? 2 : 6;

        return new Frete(Modalidade, 0, prazo);
    }
}
