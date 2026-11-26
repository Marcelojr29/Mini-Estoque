using Teste.Application.DTOs;
using Teste.Application.Exceptions;
using Teste.Application.Interfaces;
using Teste.Application.Mappings;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Interfaces;

namespace Teste.Application.Services;

/// <summary>
/// O Service ORGANIZA o caso de uso: busca os dados no repositório, chama os métodos
/// do domínio e salva. Ele não tem regra de negócio: as regras estão nas entidades.
/// </summary>
public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICalculadoraFrete _calculadoraFrete;
    private readonly IUnitOfWork _unitOfWork;

    public PedidoService(
        IPedidoRepository pedidoRepository,
        IProdutoRepository produtoRepository,
        ICalculadoraFrete calculadoraFrete,
        IUnitOfWork unitOfWork)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _calculadoraFrete = calculadoraFrete;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<PedidoResponse>> ListarAsync(StatusPedido? status)
    {
        var pedidos = await _pedidoRepository.ListarAsync(status);

        return pedidos.Select(p => PedidoMapper.ParaResponse(p)).ToList();
    }

    public async Task<PedidoResponse> ObterPorIdAsync(Guid id)
    {
        var pedido = await BuscarPedidoAsync(id);

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<PedidoResponse> CriarAsync(CriarPedidoRequest request)
    {
        var endereco = PedidoMapper.ParaEndereco(request.EnderecoEntrega);
        var pedido = new Pedido(request.Cliente, endereco, request.ModalidadeFrete);

        await _pedidoRepository.AdicionarAsync(pedido);
        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<PedidoResponse> AdicionarItemAsync(Guid pedidoId, AdicionarItemRequest request)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);
        var produto = await BuscarProdutoAsync(request.ProdutoId);

        pedido.AdicionarItem(produto, request.Quantidade);
        produto.ReservarEstoque(request.Quantidade);

        // Salva o pedido e o estoque juntos, na mesma transação
        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<PedidoResponse> RemoverItemAsync(Guid pedidoId, Guid produtoId)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);

        var itemRemovido = pedido.RemoverItem(produtoId);

        var produto = await _produtoRepository.ObterPorIdAsync(produtoId);
        if (produto != null)
            produto.DevolverEstoque(itemRemovido.Quantidade);

        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<PedidoResponse> IniciarSeparacaoAsync(Guid pedidoId)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);

        pedido.IniciarSeparacao();
        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<PedidoResponse> ConcluirSeparacaoAsync(Guid pedidoId)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);

        pedido.ConcluirSeparacao();
        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<List<FreteResponse>> CotarFreteAsync(Guid pedidoId)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);

        var cotacoes = _calculadoraFrete.CotarTodasModalidades(pedido.PesoTotalKg, pedido.EnderecoEntrega);

        return cotacoes.Select(f => PedidoMapper.ParaResponse(f)).ToList();
    }

    public async Task<PedidoResponse> DespacharAsync(Guid pedidoId)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);

        var frete = _calculadoraFrete.Calcular(pedido.ModalidadeFrete, pedido.PesoTotalKg, pedido.EnderecoEntrega);
        pedido.Despachar(frete);

        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    public async Task<PedidoResponse> CancelarAsync(Guid pedidoId)
    {
        var pedido = await BuscarPedidoAsync(pedidoId);

        pedido.Cancelar();

        // Devolve para o estoque tudo o que estava reservado no pedido
        foreach (var item in pedido.Itens)
        {
            var produto = await _produtoRepository.ObterPorIdAsync(item.ProdutoId);
            if (produto != null)
                produto.DevolverEstoque(item.Quantidade);
        }

        await _unitOfWork.CommitAsync();

        return PedidoMapper.ParaResponse(pedido);
    }

    private async Task<Pedido> BuscarPedidoAsync(Guid id)
    {
        var pedido = await _pedidoRepository.ObterPorIdAsync(id);

        if (pedido == null)
            throw new NotFoundException($"Pedido {id} não encontrado.");

        return pedido;
    }

    private async Task<Produto> BuscarProdutoAsync(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);

        if (produto == null)
            throw new NotFoundException($"Produto {id} não encontrado.");

        return produto;
    }
}
