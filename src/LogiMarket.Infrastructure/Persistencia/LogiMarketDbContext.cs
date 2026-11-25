using Microsoft.EntityFrameworkCore;
using LogiMarket.Domain.Clientes;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Domain.Pagos;

namespace LogiMarket.Infrastructure.Persistencia
{
    public class LogiMarketDbContext : DbContext
    {
        public LogiMarketDbContext(DbContextOptions<LogiMarketDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Pago> Pagos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(c => c.Identificacion);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(150);
                entity.Property(c => c.Categoria).IsRequired();
                entity.Property(c => c.Activo).IsRequired();
            });

            // Mapeo Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ClienteIdentificacion).IsRequired();
                entity.Property(p => p.Estado).IsRequired();
                entity.Property(p => p.TipoEntrega).IsRequired().HasMaxLength(50);
                
                // Mapeo del Value Object Dinero para Tarifa de Entrega
                entity.ComplexProperty(p => p.TarifaEntrega, t =>
                {
                    t.Property(d => d.Valor).HasColumnName("TarifaEntregaValor").HasColumnType("decimal(18,2)");
                });

                entity.HasMany(p => p.Lineas)
                      .WithOne()
                      .HasForeignKey("PedidoId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Mapeo PedidoDetalle
            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Sku).IsRequired().HasMaxLength(50);
                entity.Property(d => d.NombreProducto).IsRequired().HasMaxLength(150);
                entity.Property(d => d.Cantidad).IsRequired();

                entity.ComplexProperty(d => d.PrecioUnitario, t =>
                {
                    t.Property(p => p.Valor).HasColumnName("PrecioUnitarioValor").HasColumnType("decimal(18,2)");
                });
            });

            // Mapeo Pago
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.ReferenciaTransaccion).IsRequired().HasMaxLength(100);
                entity.Property(p => p.ClaveIdempotencia).IsRequired().HasMaxLength(100);
                entity.HasIndex(p => p.ClaveIdempotencia).IsUnique();

                entity.ComplexProperty(p => p.Monto, t =>
                {
                    t.Property(d => d.Valor).HasColumnName("MontoPagoValor").HasColumnType("decimal(18,2)");
                });
            });
        }
    }
}
