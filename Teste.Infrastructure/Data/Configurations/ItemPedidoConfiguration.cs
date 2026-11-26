using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teste.Domain.Entities;
using Teste.Domain.ValueObjects;

namespace Teste.Infrastructure.Data.Configurations;

public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("ItensPedido");

        builder.HasKey(i => i.Id);

        // Importante: o Id do item já vem preenchido do C#. Sem essa linha, o EF acha
        // que o item já existe no banco e tenta fazer UPDATE em vez de INSERT.
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Sku)
            .HasConversion(sku => sku.Valor, valor => new Sku(valor))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(i => i.NomeProduto).HasMaxLength(150).IsRequired();
        builder.Property(i => i.PesoUnitarioKg).HasPrecision(10, 3);

        builder.Ignore(i => i.PesoTotalKg);
    }
}
