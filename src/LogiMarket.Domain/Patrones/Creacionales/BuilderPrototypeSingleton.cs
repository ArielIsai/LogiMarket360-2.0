using System;
using System.Collections.Generic;
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Comun;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Domain.Patrones.Creacionales;

// ==========================================
// 3. PATRÓN BUILDER (Ensamblador de Pedidos)
// ==========================================
public class PedidoBuilder
{
    private string _clienteIdentificacion = string.Empty;
    private string _tipoEntrega = "ESTANDAR";
    private Dinero _tarifaEntrega = new Dinero(5.00m);
    private readonly List<(IElementoCatalogo Elemento, int Cantidad)> _items = new();

    public PedidoBuilder ConCliente(string identificacion)
    {
        _clienteIdentificacion = identificacion;
        return this;
    }

    public PedidoBuilder ConTipoEntrega(string tipoEntrega, Dinero tarifa)
    {
        _tipoEntrega = tipoEntrega;
        _tarifaEntrega = tarifa;
        return this;
    }

    public PedidoBuilder AgregarItem(IElementoCatalogo elemento, int cantidad)
    {
        if (cantidad <= 0)
            throw new ExcepcionDominio("La cantidad debe ser mayor a cero.");
        _items.Add((elemento, cantidad));
        return this;
    }

    public Pedido Construir()
    {
        if (string.IsNullOrWhiteSpace(_clienteIdentificacion))
            throw new ExcepcionDominio("No se puede construir un pedido sin cliente asignado.");
        if (_items.Count == 0)
            throw new ExcepcionDominio("No se puede construir un pedido sin líneas de productos.");

        var pedido = new Pedido(_clienteIdentificacion, _tipoEntrega, _tarifaEntrega);
        foreach (var item in _items)
        {
            pedido.AgregarLinea(item.Elemento, item.Cantidad);
        }
        return pedido;
    }
}

// ==========================================
// 4. PATRÓN PROTOTYPE (Clonación de Pedidos)
// ==========================================
public interface IPrototipo<T>
{
    T ClonarProfundo();
}

// ==========================================
// 5. PATRÓN SINGLETON (Métricas de Sesión)
// ==========================================
public sealed class MetricasSesion
{
    private static readonly Lazy<MetricasSesion> _instancia = new(() => new MetricasSesion());
    public static MetricasSesion Instancia => _instancia.Value;

    public int PedidosCreadosEnSesion { get; private set; }
    public int CobrosRealizadosEnSesion { get; private set; }
    public decimal VentaTotalAcumulada { get; private set; }

    private readonly object _bloqueo = new();

    private MetricasSesion() { }

    public void RegistrarPedido()
    {
        lock (_bloqueo) { PedidosCreadosEnSesion++; }
    }

    public void RegistrarCobro(decimal monto)
    {
        lock (_bloqueo) 
        { 
            CobrosRealizadosEnSesion++; 
            VentaTotalAcumulada += monto; 
        }
    }

    public void ReiniciarContadores()
    {
        lock (_bloqueo)
        {
            PedidosCreadosEnSesion = 0;
            CobrosRealizadosEnSesion = 0;
            VentaTotalAcumulada = 0;
        }
    }
}
