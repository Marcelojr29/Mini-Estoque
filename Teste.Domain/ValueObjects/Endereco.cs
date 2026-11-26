using Teste.Domain.Guards;

namespace Teste.Domain.ValueObjects;

/// <summary>
/// Value Object: endereço de entrega.
/// Não se "altera o número" de um endereço: troca-se o endereço inteiro.
/// </summary>
public record Endereco
{
    private static readonly string[] UfsValidas =
    {
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA",
        "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO"
    };

    private static readonly string[] UfsRegiaoSul = { "PR", "SC", "RS" };

    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Uf { get; private set; } = string.Empty;
    public string Cep { get; private set; } = string.Empty;

    // Construtor vazio usado apenas pelo Entity Framework
    private Endereco()
    {
    }

    public Endereco(string logradouro, string numero, string cidade, string uf, string cep)
    {
        Guard.AgainstNullOrEmpty(logradouro, "O logradouro é obrigatório.");
        Guard.AgainstNullOrEmpty(numero, "O número é obrigatório.");
        Guard.AgainstNullOrEmpty(cidade, "A cidade é obrigatória.");
        Guard.AgainstNullOrEmpty(uf, "A UF é obrigatória.");
        Guard.AgainstNullOrEmpty(cep, "O CEP é obrigatório.");

        uf = uf.Trim().ToUpper();
        Guard.Against(!UfsValidas.Contains(uf), "UF inválida.");

        cep = cep.Replace("-", "").Trim();
        Guard.Against(cep.Length != 8 || !cep.All(char.IsDigit), "O CEP deve conter 8 dígitos.");

        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Cidade = cidade.Trim();
        Uf = uf;
        Cep = cep;
    }

    public bool EstaNaRegiaoSul()
    {
        return UfsRegiaoSul.Contains(Uf);
    }
}
