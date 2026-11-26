namespace Teste.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma regra de negócio é violada.
/// O middleware da API transforma essa exceção em HTTP 422.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem)
    {
    }
}
