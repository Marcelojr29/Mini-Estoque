namespace Teste.Application.Exceptions;

/// <summary>Recurso não encontrado. O middleware da API transforma em HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string mensagem) : base(mensagem)
    {
    }
}
