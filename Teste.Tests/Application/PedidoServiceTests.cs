using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Teste.Application.DTOs;
using Teste.Application.Exceptions;
using Teste.Application.Services;
using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Exceptions;
using Teste.Domain.Services;
using Teste.Domain.Strategies;
using Teste.Domain.ValueObjects;
using Teste.Infrastructure.Data;
using Teste.Infrastructure.Repositories;

namespace Teste.Tests.Application;

/// <summary>
/// Testa o PedidoService com os repositórios reais e um banco SQLite em memória.
/// Cada teste recebe um banco novo e vazio.
/// </summary>
public class PedidoServiceTests : IDisposable
{
    private readonly SqliteConnection _conexao;
    private readonly AppDbContext _context;
    private readonly PedidoService _service;

    public PedidoServiceTests()
    {
        // O banco em memória existe enquanto a conexão estiver aberta
        _conexao = new SqliteConnection("DataSource=:memory:");
        _conexao.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_conexao)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        var estrategias = new List<IFreteStrategy>
        {
            new FreteEconomicoStrategy(),
            new FreteExpressoStrategy(),
            new RetiradaNaLojaStrategy()
        };

        _service = new PedidoService(
            new PedidoRepository(_context),
            new ProdutoRepository(_context),
            new CalculadoraFrete(estrategias),
            _context);
    }

    public void Dispose()
    {
        _context.Dispose();
        _conexao.Dispose();
    }

    private async Task<Produto> CadastrarProduto(int estoque)
    {
        var produto = new Produto(new Sku("VENTILADOR-40"), "Ventilador 40cm", 2m, estoque);
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    private CriarPedidoRequest CriarPedidoRequest(ModalidadeFrete modalidade)
    {
        return new CriarPedidoRequest
        {
            Cliente = "João Souza",
            ModalidadeFrete = modalidade,
            EnderecoEntrega = new EnderecoDto
            {
                Logradouro = "Av. Brasil",
                Numero = "500",
                Cidade = "Barra Velha",
                Uf = "SC",
                Cep = "88390000"
            }
        };
    }

    [Fact]
    public async Task Adicionar_item_reserva_o_estoque()
    {
        var produto = await CadastrarProduto(10);
        var pedido = await _service.CriarAsync(CriarPedidoRequest(ModalidadeFrete.Economico));

        var resultado = await _service.AdicionarItemAsync(pedido.Id,
            new AdicionarItemRequest { ProdutoId = produto.Id, Quantidade = 3 });

        Assert.Single(resultado.Itens);
        Assert.Equal(7, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public async Task Estoque_insuficiente_lanca_excecao()
    {
        var produto = await CadastrarProduto(2);
        var pedido = await _service.CriarAsync(CriarPedidoRequest(ModalidadeFrete.Economico));

        await Assert.ThrowsAsync<DomainException>(() => _service.AdicionarItemAsync(pedido.Id,
            new AdicionarItemRequest { ProdutoId = produto.Id, Quantidade = 5 }));
    }

    [Fact]
    public async Task Cancelar_pedido_devolve_o_estoque()
    {
        var produto = await CadastrarProduto(10);
        var pedido = await _service.CriarAsync(CriarPedidoRequest(ModalidadeFrete.Economico));
        await _service.AdicionarItemAsync(pedido.Id, new AdicionarItemRequest { ProdutoId = produto.Id, Quantidade = 4 });

        var resultado = await _service.CancelarAsync(pedido.Id);

        Assert.Equal(StatusPedido.Cancelado, resultado.Status);
        Assert.Equal(10, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public async Task Despachar_calcula_o_frete_pela_modalidade_do_pedido()
    {
        var produto = await CadastrarProduto(10);
        var pedido = await _service.CriarAsync(CriarPedidoRequest(ModalidadeFrete.Expresso));
        await _service.AdicionarItemAsync(pedido.Id, new AdicionarItemRequest { ProdutoId = produto.Id, Quantidade = 5 });
        await _service.IniciarSeparacaoAsync(pedido.Id);
        await _service.ConcluirSeparacaoAsync(pedido.Id);

        var resultado = await _service.DespacharAsync(pedido.Id);

        Assert.Equal(StatusPedido.Despachado, resultado.Status);
        Assert.NotNull(resultado.Frete);
        Assert.Equal(79.00m, resultado.Frete.Valor); // 30 + (10kg * 4,90)
        Assert.Equal(1, resultado.Frete.PrazoEmDias); // SC fica na região Sul
    }

    [Fact]
    public async Task Pedido_inexistente_lanca_NotFoundException()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.ObterPorIdAsync(Guid.NewGuid()));
    }
}
