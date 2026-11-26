using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Teste.Domain.Entities;

namespace Teste.Infrastructure.Data.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Cliente).HasMaxLength(150).IsRequired();

        // Salva os enums como texto ("EmSeparacao") em vez de número (2)
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.ModalidadeFrete).HasConversion<string>().HasMaxLength(20);

        // Endereço é um Value Object: as colunas ficam na própria tabela Pedidos
        // (EnderecoEntrega_Cidade, EnderecoEntrega_Uf...), sem criar outra tabela.
        builder.OwnsOne(p => p.EnderecoEntrega, endereco =>
        {
            endereco.Property(e => e.Logradouro).HasMaxLength(200);
            endereco.Property(e => e.Numero).HasMaxLength(20);
            endereco.Property(e => e.Cidade).HasMaxLength(100);
            endereco.Property(e => e.Uf).HasMaxLength(2);
            endereco.Property(e => e.Cep).HasMaxLength(8);
        });

        // Frete também é um Value Object, e só é preenchido quando o pedido é despachado.
        builder.OwnsOne(p => p.Frete, frete =>
        {
            frete.Property(f => f.Modalidade).HasConversion<string>().HasMaxLength(20);
            frete.Property(f => f.Valor).HasPrecision(10, 2);
        });

        // Um pedido tem vários itens. Se o pedido for apagado, os itens também são.
        builder.HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey("PedidoId")
            .OnDelete(DeleteBehavior.Cascade);

        // Diz ao EF para usar a lista privada _itens (a propriedade Itens é só leitura).
        builder.Navigation(p => p.Itens).HasField("_itens");

        // Propriedade calculada, não vira coluna no banco.
        builder.Ignore(p => p.PesoTotalKg);
    }
}
