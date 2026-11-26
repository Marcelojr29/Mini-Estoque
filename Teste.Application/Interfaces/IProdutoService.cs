using Teste.Application.DTOs;

namespace Teste.Application.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoResponse>> ListarAsync();
    Task<ProdutoResponse> ObterPorIdAsync(Guid id);
    Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request);
    Task<ProdutoResponse> DarEntradaEstoqueAsync(Guid id, EntradaEstoqueRequest request);
}
