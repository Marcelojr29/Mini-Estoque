using System.Text.RegularExpressions;
using Teste.Domain.Guards;

namespace Teste.Domain.ValueObjects;

/// <summary>
/// Value Object: código do produto (SKU).
/// Value Object não tem Id: dois SKUs com o mesmo valor são iguais.
/// Ele é imutável e se valida no construtor, então não existe SKU inválido no sistema.
/// O "record" do C# já compara os objetos pelo valor.
/// </summary>
public record Sku
{
    public string Valor { get; }

    public Sku(string valor)
    {
        Guard.AgainstNullOrEmpty(valor, "O SKU é obrigatório.");

        valor = valor.Trim().ToUpper();

        Guard.Against(!Regex.IsMatch(valor, "^[A-Z0-9-]{3,20}$"),
            "SKU inválido. Use de 3 a 20 caracteres: letras, números ou hífen.");

        Valor = valor;
    }

    public override string ToString()
    {
        return Valor;
    }
}
