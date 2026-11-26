using Teste.Application.DTOs;
using Teste.Domain.Enums;

namespace Teste.Application.Interfaces;

public interface IPedidoService
{
    Task<List<PedidoResponse>> ListarAsync(StatusPedido? status);
    Task<PedidoResponse> ObterPorIdAsync(Guid id);
    Task<PedidoResponse> CriarAsync(CriarPedidoRequest request);
    Task<PedidoResponse> AdicionarItemAsync(Guid pedidoId, AdicionarItemRequest request);
    Task<PedidoResponse> RemoverItemAsync(Guid pedidoId, Guid produtoId);
    Task<PedidoResponse> IniciarSeparacaoAsync(Guid pedidoId);
    Task<PedidoResponse> ConcluirSeparacaoAsync(Guid pedidoId);
    Task<List<FreteResponse>> CotarFreteAsync(Guid pedidoId);
    Task<PedidoResponse> DespacharAsync(Guid pedidoId);
    Task<PedidoResponse> CancelarAsync(Guid pedidoId);
}
