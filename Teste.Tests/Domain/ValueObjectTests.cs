using Teste.Domain.Exceptions;
using Teste.Domain.ValueObjects;

namespace Teste.Tests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void Sku_e_convertido_para_maiusculas()
    {
        var sku = new Sku("  cama-queen-001 ");

        Assert.Equal("CAMA-QUEEN-001", sku.Valor);
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("SKU COM ESPACO")]
    [InlineData("SKU-MUITO-GRANDE-DEMAIS-123")]
    public void Sku_invalido_lanca_excecao(string valor)
    {
        Assert.Throws<DomainException>(() => new Sku(valor));
    }

    [Fact]
    public void Enderecos_com_os_mesmos_valores_sao_iguais()
    {
        var endereco1 = new Endereco("Rua A", "10", "Brusque", "SC", "88350-000");
        var endereco2 = new Endereco("Rua A", "10", "Brusque", "sc", "88350000");

        // Value Objects são comparados pelo valor, não pela referência
        Assert.Equal(endereco1, endereco2);
    }

    [Fact]
    public void Endereco_com_uf_invalida_lanca_excecao()
    {
        Assert.Throws<DomainException>(() => new Endereco("Rua A", "10", "Cidade", "XX", "88350000"));
    }

    [Fact]
    public void Endereco_com_cep_invalido_lanca_excecao()
    {
        Assert.Throws<DomainException>(() => new Endereco("Rua A", "10", "Cidade", "SC", "123"));
    }
}
