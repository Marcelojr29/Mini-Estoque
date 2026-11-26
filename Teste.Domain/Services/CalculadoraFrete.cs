using Teste.Domain.Enums;
using Teste.Domain.Exceptions;
using Teste.Domain.Interfaces;
using Teste.Domain.Strategies;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Services;

/// <summary>
/// Domain Service: regra de negócio que não pertence a uma entidade específica.
/// Recebe todas as estratégias de frete (pela injeção de dependência) e
/// escolhe a estratégia certa de acordo com a modalidade.
/// </summary>
public class CalculadoraFrete : ICalculadoraFrete
{
    private readonly IEnumerable<IFreteStrategy> _estrategias;

    public CalculadoraFrete(IEnumerable<IFreteStrategy> estrategias)
    {
        _estrategias = estrategias;
    }

    public Frete Calcular(ModalidadeFrete modalidade, decimal pesoTotalKg, Endereco destino)
    {
        var estrategia = _estrategias.FirstOrDefault(e => e.Modalidade == modalidade);

        if (estrategia == null)
            throw new DomainException($"Não existe regra de frete para a modalidade {modalidade}.");

        return estrategia.Calcular(pesoTotalKg, destino);
    }

    public List<Frete> CotarTodasModalidades(decimal pesoTotalKg, Endereco destino)
    {
        var cotacoes = new List<Frete>();

        foreach (var estrategia in _estrategias)
        {
            cotacoes.Add(estrategia.Calcular(pesoTotalKg, destino));
        }

        return cotacoes.OrderBy(f => f.Valor).ToList();
    }
}
