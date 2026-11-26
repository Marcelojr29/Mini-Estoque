using Teste.Domain.Enums;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Strategies;

/// <summary>
/// Strategy Pattern: cada modalidade de frete tem a sua regra de cálculo em uma classe separada.
/// Para criar uma nova modalidade, basta criar uma nova classe e registrar na injeção
/// de dependência, sem alterar as classes que já existem (o "O" do SOLID).
/// Isso evita um switch/if gigante.
/// </summary>
public interface IFreteStrategy
{
    ModalidadeFrete Modalidade { get; }
    Frete Calcular(decimal pesoTotalKg, Endereco destino);
}
