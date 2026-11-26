using System.ComponentModel.DataAnnotations;
using Teste.Domain.Enums;

namespace Teste.Application.DTOs;

public class EnderecoDto
{
    [Required(ErrorMessage = "O logradouro é obrigatório.")]
    public string Logradouro { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número é obrigatório.")]
    public string Numero { get; set; } = string.Empty;

    [Required(ErrorMessage = "A cidade é obrigatória.")]
    public string Cidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "A UF é obrigatória.")]
    public string Uf { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CEP é obrigatório.")]
    public string Cep { get; set; } = string.Empty;
}

public class CriarPedidoRequest
{
    [Required(ErrorMessage = "O cliente é obrigatório.")]
    [MaxLength(150, ErrorMessage = "O cliente deve ter no máximo 150 caracteres.")]
    public string Cliente { get; set; } = string.Empty;

    public EnderecoDto EnderecoEntrega { get; set; } = new EnderecoDto();

    public ModalidadeFrete ModalidadeFrete { get; set; }
}

public class AdicionarItemRequest
{
    public Guid ProdutoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
    public int Quantidade { get; set; }
}

public class ItemPedidoResponse
{
    public Guid ProdutoId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PesoUnitarioKg { get; set; }
    public decimal PesoTotalKg { get; set; }
}

public class FreteResponse
{
    public ModalidadeFrete Modalidade { get; set; }
    public decimal Valor { get; set; }
    public int PrazoEmDias { get; set; }
}

public class PedidoResponse
{
    public Guid Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public EnderecoDto EnderecoEntrega { get; set; } = new EnderecoDto();
    public ModalidadeFrete ModalidadeFrete { get; set; }
    public StatusPedido Status { get; set; }
    public decimal PesoTotalKg { get; set; }
    public FreteResponse? Frete { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? DespachadoEm { get; set; }
    public List<ItemPedidoResponse> Itens { get; set; } = new List<ItemPedidoResponse>();
}
