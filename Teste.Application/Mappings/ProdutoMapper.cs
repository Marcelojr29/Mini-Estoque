using Teste.Application.DTOs;
using Teste.Domain.Entities;

namespace Teste.Application.Mappings;

/// <summary>Converte a entidade Produto para o DTO de resposta.</summary>
public static class ProdutoMapper
{
    public static ProdutoResponse ParaResponse(Produto produto)
    {
        return new ProdutoResponse
        {
            Id = produto.Id,
            Sku = produto.Sku.Valor,
            Nome = produto.Nome,
            PesoKg = produto.PesoKg,
            QuantidadeEmEstoque = produto.QuantidadeEmEstoque
        };
    }
}
