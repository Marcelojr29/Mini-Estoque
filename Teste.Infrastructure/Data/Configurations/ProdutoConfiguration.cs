using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teste.Domain.Entities;
using Teste.Domain.ValueObjects;

namespace Teste.Infrastructure.Data.Configurations;

/// <summary>
/// Mapeamento da tabela usando Fluent API. Assim as entidades do Domain ficam
/// limpas, sem atributos de banco de dados como [Key] ou [Table].
/// </summary>
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        // O Id já é gerado no C# (Guid.NewGuid), então o banco não precisa gerar.
        builder.Property(p => p.Id).ValueGeneratedNever();

        // O SKU é um Value Object: no banco ele é salvo como uma coluna de texto.
        builder.Property(p => p.Sku)
            .HasConversion(sku => sku.Valor, valor => new Sku(valor))
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => p.Sku).IsUnique();

        builder.Property(p => p.Nome).HasMaxLength(150).IsRequired();
        builder.Property(p => p.PesoKg).HasPrecision(10, 3);
    }
}
