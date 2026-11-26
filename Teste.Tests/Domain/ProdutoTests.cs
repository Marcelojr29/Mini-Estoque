using Teste.Domain.Entities;
using Teste.Domain.Exceptions;
using Teste.Domain.ValueObjects;

namespace Teste.Tests.Domain;

public class ProdutoTests
{
    private Produto CriarProduto(int estoque)
    {
        return new Produto(new Sku("PANELA-5L"), "Panela de Pressão 5L", 2.4m, estoque);
    }

    [Fact]
    public void Reservar_estoque_diminui_o_saldo()
    {
        var produto = CriarProduto(10);

        produto.ReservarEstoque(4);

        Assert.Equal(6, produto.QuantidadeEmEstoque);
    }

    [Fact]
    public void Nao_deve_reservar_mais_do_que_o_disponivel()
    {
        var produto = CriarProduto(3);

        var erro = Assert.Throws<DomainException>(() => produto.ReservarEstoque(5));

        Assert.Contains("Estoque insuficiente", erro.Message);
        Assert.Equal(3, produto.QuantidadeEmEstoque);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Entrada_de_estoque_exige_quantidade_positiva(int quantidade)
    {
        var produto = CriarProduto(0);

        Assert.Throws<DomainException>(() => produto.DarEntradaEstoque(quantidade));
    }

    [Fact]
    public void Nao_deve_criar_produto_com_peso_zero()
    {
        Assert.Throws<DomainException>(() => new Produto(new Sku("ABC-1"), "Produto", 0m, 10));
    }
}
