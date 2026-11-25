using System;
using System.Collections.Generic;
using System.Linq;
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Pedidos;

public enum EstadoPedido
{
    Borrador = 0,
    Validado = 1,
    Pagado = 2,
    Despachado = 3,
    Entregado = 4,
    Cancelado = 5
}

public class PedidoDetalle
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string NombreProducto { get; private set; } = string.Empty;
    public Dinero PrecioUnitario { get; private set; }
    public int Cantidad { get; private set; }
    public Dinero Subtotal => new Dinero(PrecioUnitario.Valor * Cantidad);

    protected PedidoDetalle() { }

    public PedidoDetalle(string sku, string nombreProducto, Dinero precioUnitario, int cantidad)
    {
        if (cantidad <= 0)
            throw new ExcepcionDominio("La cantidad de la línea debe ser mayor a cero.");
        
        Id = Guid.NewGuid();
        Sku = sku;
        NombreProducto = nombreProducto;
        PrecioUnitario = precioUnitario;
        Cantidad = cantidad;
    }
}

public class Pedido
{
    public Guid Id { get; private set; }
    public string ClienteIdentificacion { get; private set; } = string.Empty;
    public EstadoPedido Estado { get; private set; }
    public string TipoEntrega { get; private set; } = "ESTANDAR"; // ESTANDAR o EXPRES
    public Dinero TarifaEntrega { get; private set; }
    public string? ReferenciaPago { get; private set; }
    public string? GuiaDespacho { get; private set; }
    public DateTime FechaCreacion { get; private set; }

    private readonly List<PedidoDetalle> _lineas = new();
    public IReadOnlyCollection<PedidoDetalle> Lineas => _lineas.AsReadOnly();

    public int UnidadesFisicasTotales => _lineas.Sum(l => l.Cantidad);

    public Dinero Subtotal => new Dinero(_lineas.Sum(l => l.Subtotal.Valor));

    protected Pedido() { }

    public Pedido(string clienteIdentificacion, string tipoEntrega, Dinero tarifaEntrega)
    {
        if (string.IsNullOrWhiteSpace(clienteIdentificacion))
            throw new ExcepcionDominio("El pedido debe estar asociado a un cliente.");

        Id = Guid.NewGuid();
        ClienteIdentificacion = clienteIdentificacion.Trim();
        TipoEntrega = tipoEntrega;
        TarifaEntrega = tarifaEntrega;
        Estado = EstadoPedido.Borrador;
        FechaCreacion = DateTime.UtcNow;
    }

    public void AgregarLinea(IElementoCatalogo elemento, int cantidad)
    {
        if (Estado != EstadoPedido.Borrador)
            throw new ExcepcionDominio("Solo se pueden modificar líneas en pedidos con estado BORRADOR.");

        // Validamos si es un kit para expandir o producto simple
        foreach (var componente in elemento.ExpandirComponentes())
        {
            // Verificamos si ya existe la línea para sumar la cantidad
            var existente = _lineas.FirstOrDefault(l => l.Sku == componente.Sku);
            if (existente != null)
            {
                // Reemplazamos con nueva cantidad acumulada
                _lineas.Remove(existente);
                _lineas.Add(new PedidoDetalle(componente.Sku, componente.Sku, elemento.Precio, existente.Cantidad + (componente.Cantidad * cantidad)));
            }
            else
            {
                _lineas.Add(new PedidoDetalle(componente.Sku, componente.Sku, elemento.Precio, componente.Cantidad * cantidad));
            }
        }
    }

    public void Validar()
    {
        if (Estado != EstadoPedido.Borrador)
            throw new ExcepcionDominio("El pedido solo puede validarse desde el estado BORRADOR.");
        if (!_lineas.Any())
            throw new ExcepcionDominio("No se puede validar un pedido sin líneas de productos.");

        Estado = EstadoPedido.Validado;
    }

    public void MarcarComoPagado(string referenciaPago)
    {
        if (Estado != EstadoPedido.Validado)
            throw new ExcepcionDominio("El pedido debe estar VALIDADO para proceder al pago.");
        if (string.IsNullOrWhiteSpace(referenciaPago))
            throw new ExcepcionDominio("Se requiere una referencia de transacción válida.");

        ReferenciaPago = referenciaPago;
        Estado = EstadoPedido.Pagado;
    }

    public void MarcarComoDespachado(string guia)
    {
        if (Estado != EstadoPedido.Pagado)
            throw new ExcepcionDominio("Solo se pueden despachar pedidos que se encuentren PAGADOS.");
        
        GuiaDespacho = guia;
        Estado = EstadoPedido.Despachado;
    }

    public void MarcarComoEntregado()
    {
        if (Estado != EstadoPedido.Despachado)
            throw new ExcepcionDominio("El pedido debe estar DESPACHADO para marcarse como entregado.");

        Estado = EstadoPedido.Entregado;
    }

    public void Cancelar()
    {
        if (Estado != EstadoPedido.Borrador && Estado != EstadoPedido.Validado)
            throw new ExcepcionDominio("Solo se pueden cancelar pedidos en estado BORRADOR o VALIDADO.");

        Estado = EstadoPedido.Cancelado;
    }
}
