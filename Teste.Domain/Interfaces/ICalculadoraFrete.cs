using Teste.Domain.Enums;
using Teste.Domain.ValueObjects;

namespace Teste.Domain.Interfaces;

public interface ICalculadoraFrete
{
    Frete Calcular(ModalidadeFrete modalidade, decimal pesoTotalKg, Endereco destino);
    List<Frete> CotarTodasModalidades(decimal pesoTotalKg, Endereco destino);
}
