using Teste.Application.DTOs;
using Teste.Application.Exceptions;
using Teste.Application.Interfaces;
using Teste.Application.Mappings;
using Teste.Domain.Entities;
using Teste.Domain.Exceptions;
using Teste.Domain.Interfaces;
using Teste.Domain.ValueObjects;

namespace Teste.Application.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProdutoService(IProdutoRepository produtoRepository, IUnitOfWork unitOfWork)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProdutoResponse>> ListarAsync()
    {
        var produtos = await _produtoRepository.ListarAsync();

        return produtos.Select(p => ProdutoMapper.ParaResponse(p)).ToList();
    }

    public async Task<ProdutoResponse> ObterPorIdAsync(Guid id)
    {
        var produto = await BuscarProdutoAsync(id);

        return ProdutoMapper.ParaResponse(produto);
    }

    public async Task<ProdutoResponse> CriarAsync(CriarProdutoRequest request)
    {
        var sku = new Sku(request.Sku);

        var skuJaExiste = await _produtoRepository.ExisteSkuAsync(sku);
        if (skuJaExiste)
            throw new DomainException($"Já existe um produto cadastrado com o SKU {sku}.");

        var produto = new Produto(sku, request.Nome, request.PesoKg, request.EstoqueInicial);

        await _produtoRepository.AdicionarAsync(produto);
        await _unitOfWork.CommitAsync();

        return ProdutoMapper.ParaResponse(produto);
    }

    public async Task<ProdutoResponse> DarEntradaEstoqueAsync(Guid id, EntradaEstoqueRequest request)
    {
        var produto = await BuscarProdutoAsync(id);

        produto.DarEntradaEstoque(request.Quantidade);
        await _unitOfWork.CommitAsync();

        return ProdutoMapper.ParaResponse(produto);
    }

    private async Task<Produto> BuscarProdutoAsync(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);

        if (produto == null)
            throw new NotFoundException($"Produto {id} não encontrado.");

        return produto;
    }
}
