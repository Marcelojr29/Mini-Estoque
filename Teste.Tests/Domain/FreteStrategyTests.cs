using Teste.Domain.Enums;
using Teste.Domain.Services;
using Teste.Domain.Strategies;
using Teste.Domain.ValueObjects;

namespace Teste.Tests.Domain;

public class FreteStrategyTests
{
    private readonly Endereco _destinoSul = new Endereco("Rua A", "1", "Joinville", "SC", "89200000");
    private readonly Endereco _destinoSudeste = new Endereco("Rua B", "2", "São Paulo", "SP", "01000000");

    private CalculadoraFrete CriarCalculadora()
    {
        var estrategias = new List<IFreteStrategy>
        {
            new FreteEconomicoStrategy(),
            new FreteExpressoStrategy(),
            new RetiradaNaLojaStrategy()
        };

        return new CalculadoraFrete(estrategias);
    }

    [Fact]
    public void Frete_economico_para_o_sul()
    {
        var frete = new FreteEconomicoStrategy().Calcular(10m, _destinoSul);

        Assert.Equal(40.00m, frete.Valor); // 15 + (10kg * 2,50)
        Assert.Equal(5, frete.PrazoEmDias);
    }

    [Fact]
    public void Frete_expresso_para_fora_do_sul()
    {
        var frete = new FreteExpressoStrategy().Calcular(10m, _destinoSudeste);

        Assert.Equal(79.00m, frete.Valor); // 30 + (10kg * 4,90)
        Assert.Equal(3, frete.PrazoEmDias);
    }

    [Fact]
    public void Retirada_na_loja_nao_tem_custo()
    {
        var frete = new RetiradaNaLojaStrategy().Calcular(50m, _destinoSul);

        Assert.Equal(0m, frete.Valor);
    }

    [Theory]
    [InlineData(ModalidadeFrete.Economico)]
    [InlineData(ModalidadeFrete.Expresso)]
    [InlineData(ModalidadeFrete.RetiradaNaLoja)]
    public void Calculadora_usa_a_estrategia_da_modalidade(ModalidadeFrete modalidade)
    {
        var frete = CriarCalculadora().Calcular(modalidade, 5m, _destinoSul);

        Assert.Equal(modalidade, frete.Modalidade);
    }

    [Fact]
    public void Cotacao_retorna_as_tres_modalidades()
    {
        var cotacoes = CriarCalculadora().CotarTodasModalidades(5m, _destinoSul);

        Assert.Equal(3, cotacoes.Count);
    }
}
