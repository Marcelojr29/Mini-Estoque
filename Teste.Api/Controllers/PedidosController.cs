using Microsoft.AspNetCore.Mvc;
using Teste.Application.DTOs;
using Teste.Application.Interfaces;
using Teste.Domain.Enums;

namespace Teste.Api.Controllers;

/// <summary>
/// Cada mudança de status tem o seu próprio endpoint com o nome da ação do negócio
/// (iniciar-separacao, despachar...). Assim não dá para "pular" etapas,
/// porque quem valida se a mudança é permitida é a entidade Pedido.
/// </summary>
[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidosController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PedidoResponse>>> Listar([FromQuery] StatusPedido? status)
    {
        var pedidos = await _pedidoService.ListarAsync(status);
        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PedidoResponse>> ObterPorId(Guid id)
    {
        var pedido = await _pedidoService.ObterPorIdAsync(id);
        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<PedidoResponse>> Criar(CriarPedidoRequest request)
    {
        var pedido = await _pedidoService.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }

    [HttpPost("{id}/itens")]
    public async Task<ActionResult<PedidoResponse>> AdicionarItem(Guid id, AdicionarItemRequest request)
    {
        var pedido = await _pedidoService.AdicionarItemAsync(id, request);
        return Ok(pedido);
    }

    [HttpDelete("{id}/itens/{produtoId}")]
    public async Task<ActionResult<PedidoResponse>> RemoverItem(Guid id, Guid produtoId)
    {
        var pedido = await _pedidoService.RemoverItemAsync(id, produtoId);
        return Ok(pedido);
    }

    [HttpPost("{id}/iniciar-separacao")]
    public async Task<ActionResult<PedidoResponse>> IniciarSeparacao(Guid id)
    {
        var pedido = await _pedidoService.IniciarSeparacaoAsync(id);
        return Ok(pedido);
    }

    [HttpPost("{id}/concluir-separacao")]
    public async Task<ActionResult<PedidoResponse>> ConcluirSeparacao(Guid id)
    {
        var pedido = await _pedidoService.ConcluirSeparacaoAsync(id);
        return Ok(pedido);
    }

    [HttpGet("{id}/cotacoes-frete")]
    public async Task<ActionResult<List<FreteResponse>>> CotarFrete(Guid id)
    {
        var cotacoes = await _pedidoService.CotarFreteAsync(id);
        return Ok(cotacoes);
    }

    [HttpPost("{id}/despachar")]
    public async Task<ActionResult<PedidoResponse>> Despachar(Guid id)
    {
        var pedido = await _pedidoService.DespacharAsync(id);
        return Ok(pedido);
    }

    [HttpPost("{id}/cancelar")]
    public async Task<ActionResult<PedidoResponse>> Cancelar(Guid id)
    {
        var pedido = await _pedidoService.CancelarAsync(id);
        return Ok(pedido);
    }
}
