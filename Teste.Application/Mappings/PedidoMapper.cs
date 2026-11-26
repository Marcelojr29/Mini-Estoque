using Teste.Application.DTOs;
using Teste.Domain.Entities;
using Teste.Domain.ValueObjects;

namespace Teste.Application.Mappings;

/// <summary>
/// Converte entre as entidades de domínio e os DTOs.
/// Mapeamento manual: simples, explícito e fácil de depurar.
/// </summary>
public static class PedidoMapper
{
    public static PedidoResponse ParaResponse(Pedido pedido)
    {
        var response = new PedidoResponse
        {
            Id = pedido.Id,
            Cliente = pedido.Cliente,
            EnderecoEntrega = ParaDto(pedido.EnderecoEntrega),
            ModalidadeFrete = pedido.ModalidadeFrete,
            Status = pedido.Status,
            PesoTotalKg = pedido.PesoTotalKg,
            CriadoEm = pedido.CriadoEm,
            DespachadoEm = pedido.DespachadoEm
        };

        if (pedido.Frete != null)
            response.Frete = ParaResponse(pedido.Frete);

        foreach (var item in pedido.Itens)
        {
            response.Itens.Add(new ItemPedidoResponse
            {
                ProdutoId = item.ProdutoId,
                Sku = item.Sku.Valor,
                NomeProduto = item.NomeProduto,
                Quantidade = item.Quantidade,
                PesoUnitarioKg = item.PesoUnitarioKg,
                PesoTotalKg = item.PesoTotalKg
            });
        }

        return response;
    }

    public static FreteResponse ParaResponse(Frete frete)
    {
        return new FreteResponse
        {
            Modalidade = frete.Modalidade,
            Valor = frete.Valor,
            PrazoEmDias = frete.PrazoEmDias
        };
    }

    public static EnderecoDto ParaDto(Endereco endereco)
    {
        return new EnderecoDto
        {
            Logradouro = endereco.Logradouro,
            Numero = endereco.Numero,
            Cidade = endereco.Cidade,
            Uf = endereco.Uf,
            Cep = endereco.Cep
        };
    }

    public static Endereco ParaEndereco(EnderecoDto dto)
    {
        return new Endereco(dto.Logradouro, dto.Numero, dto.Cidade, dto.Uf, dto.Cep);
    }
}
