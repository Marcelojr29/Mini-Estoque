using Teste.Domain.Exceptions;

namespace Teste.Domain.Guards;

/// <summary>
/// Guard Clauses: validações feitas no início dos métodos para impedir que
/// um objeto fique em um estado inválido. Se a validação falhar, lança uma
/// DomainException com a mensagem informada.
/// </summary>
public static class Guard
{
    public static void AgainstNullOrEmpty(string? valor, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException(mensagem);
    }

    public static void AgainstNull(object? valor, string mensagem)
    {
        if (valor == null)
            throw new DomainException(mensagem);
    }

    public static void AgainstMaxLength(string valor, int tamanhoMaximo, string mensagem)
    {
        if (valor.Length > tamanhoMaximo)
            throw new DomainException(mensagem);
    }

    public static void AgainstZeroOrNegative(decimal valor, string mensagem)
    {
        if (valor <= 0)
            throw new DomainException(mensagem);
    }

    public static void AgainstNegative(decimal valor, string mensagem)
    {
        if (valor < 0)
            throw new DomainException(mensagem);
    }

    /// <summary>Guard genérico: lança a exceção se a condição for verdadeira.</summary>
    public static void Against(bool condicao, string mensagem)
    {
        if (condicao)
            throw new DomainException(mensagem);
    }
}
