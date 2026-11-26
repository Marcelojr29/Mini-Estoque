using Teste.Domain.Entities;
using Teste.Domain.Enums;
using Teste.Domain.Exceptions;
using Teste.Domain.ValueObjects;

namespace Teste.Tests.Domain;

// Testes do domínio: não precisam de banco de dados nem da API, só das classes C#.
public class PedidoTests
{
    private Endereco CriarEndereco()
    {
        return new Endereco("Rua das Palmeiras", "100", "Barra Velha", "SC", "88390-000");
    }

    private Pedido CriarPedido(ModalidadeFrete modalidade)
    {
        return new Pedido("Maria Silva", CriarEndereco(), modalidade);
    }

    private Produto CriarProduto(string sku)
    {
        return new Produto(new Sku(sku), "Toalha de Banho", 0.5m, 10);
    }

    [Fact]
    public void Novo_pedido_comeca_com_status_Criado_e_sem_itens()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);

        Assert.Equal(StatusPedido.Criado, pedido.Status);
        Assert.Empty(pedido.Itens);
        Assert.Null(pedido.Frete);
    }

    [Fact]
    public void Nao_deve_criar_pedido_sem_cliente()
    {
        var erro = Assert.Throws<DomainException>(() => new Pedido("", CriarEndereco(), ModalidadeFrete.Economico));

        Assert.Equal("O cliente é obrigatório.", erro.Message);
    }

    [Fact]
    public void Adicionar_o_mesmo_produto_duas_vezes_soma_a_quantidade()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);
        var produto = CriarProduto("TOALHA-01");

        pedido.AdicionarItem(produto, 2);
        pedido.AdicionarItem(produto, 3);

        var item = Assert.Single(pedido.Itens);
        Assert.Equal(5, item.Quantidade);
        Assert.Equal(2.5m, pedido.PesoTotalKg);
    }

    [Fact]
    public void Nao_deve_iniciar_separacao_de_pedido_sem_itens()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);

        Assert.Throws<DomainException>(() => pedido.IniciarSeparacao());
    }

    [Fact]
    public void Nao_deve_adicionar_itens_depois_de_iniciar_a_separacao()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);
        pedido.AdicionarItem(CriarProduto("TOALHA-01"), 1);
        pedido.IniciarSeparacao();

        Assert.Throws<DomainException>(() => pedido.AdicionarItem(CriarProduto("OUTRO-01"), 1));
    }

    [Fact]
    public void Nao_deve_despachar_sem_separar()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);
        pedido.AdicionarItem(CriarProduto("TOALHA-01"), 1);

        var frete = new Frete(ModalidadeFrete.Economico, 20m, 5);

        Assert.Throws<DomainException>(() => pedido.Despachar(frete));
    }

    [Fact]
    public void Fluxo_completo_ate_despachar()
    {
        var pedido = CriarPedido(ModalidadeFrete.Expresso);
        pedido.AdicionarItem(CriarProduto("TOALHA-01"), 2);

        pedido.IniciarSeparacao();
        pedido.ConcluirSeparacao();
        pedido.Despachar(new Frete(ModalidadeFrete.Expresso, 34.90m, 1));

        Assert.Equal(StatusPedido.Despachado, pedido.Status);
        Assert.NotNull(pedido.Frete);
        Assert.NotNull(pedido.DespachadoEm);
    }

    [Fact]
    public void Nao_deve_despachar_com_frete_de_outra_modalidade()
    {
        var pedido = CriarPedido(ModalidadeFrete.Expresso);
        pedido.AdicionarItem(CriarProduto("TOALHA-01"), 1);
        pedido.IniciarSeparacao();
        pedido.ConcluirSeparacao();

        var freteEconomico = new Frete(ModalidadeFrete.Economico, 10m, 5);

        Assert.Throws<DomainException>(() => pedido.Despachar(freteEconomico));
    }

    [Fact]
    public void Nao_deve_cancelar_pedido_ja_despachado()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);
        pedido.AdicionarItem(CriarProduto("TOALHA-01"), 1);
        pedido.IniciarSeparacao();
        pedido.ConcluirSeparacao();
        pedido.Despachar(new Frete(ModalidadeFrete.Economico, 16.25m, 5));

        Assert.Throws<DomainException>(() => pedido.Cancelar());
    }

    [Fact]
    public void Remover_item_que_nao_esta_no_pedido_lanca_excecao()
    {
        var pedido = CriarPedido(ModalidadeFrete.Economico);

        Assert.Throws<DomainException>(() => pedido.RemoverItem(Guid.NewGuid()));
    }
}
