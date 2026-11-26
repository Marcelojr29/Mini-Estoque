using Microsoft.AspNetCore.Mvc;
using Teste.Application.DTOs;
using Teste.Application.Interfaces;

namespace Teste.Api.Controllers;

/// <summary>
/// O Controller só recebe a requisição, chama o Service e devolve a resposta.
/// Nenhuma regra de negócio fica aqui.
/// </summary>
[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProdutoResponse>>> Listar()
    {
        var produtos = await _produtoService.ListarAsync();
        return Ok(produtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(Guid id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponse>> Criar(CriarProdutoRequest request)
    {
        var produto = await _produtoService.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    [HttpPost("{id}/entrada-estoque")]
    public async Task<ActionResult<ProdutoResponse>> DarEntradaEstoque(Guid id, EntradaEstoqueRequest request)
    {
        var produto = await _produtoService.DarEntradaEstoqueAsync(id, request);
        return Ok(produto);
    }
}
