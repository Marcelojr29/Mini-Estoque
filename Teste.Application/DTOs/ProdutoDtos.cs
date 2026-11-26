using System.ComponentModel.DataAnnotations;

namespace Teste.Application.DTOs;

// DTOs: o formato dos dados que entram e saem da API.
// A entidade de domínio nunca é exposta diretamente.
// Os atributos [Required] e [Range] validam a ENTRADA (parecido com o class-validator do NestJS).
// As regras de NEGÓCIO continuam nas entidades do Domain.

public class CriarProdutoRequest
{
    [Required(ErrorMessage = "O SKU é obrigatório.")]
    public string Sku { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(0.001, 10000, ErrorMessage = "O peso deve ser maior que zero.")]
    public decimal PesoKg { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "O estoque inicial não pode ser negativo.")]
    public int EstoqueInicial { get; set; }
}

public class EntradaEstoqueRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }
}

public class ProdutoResponse
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal PesoKg { get; set; }
    public int QuantidadeEmEstoque { get; set; }
}
