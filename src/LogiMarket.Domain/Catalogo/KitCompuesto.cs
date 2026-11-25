using System.Collections.Generic;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Catalogo;

public interface IElementoCatalogo
{
    string Sku { get; }
    string Nombre { get; }
    Dinero Precio { get; }
    decimal PesoKg { get; }
    int ObtenerStockMinimo();
    IEnumerable<(string Sku, int Cantidad)> ExpandirComponentes();
}

public class ProductoSimple : IElementoCatalogo
{
    public string Sku { get; }
    public string Nombre { get; }
    public Dinero Precio { get; }
    public decimal PesoKg { get; }
    public int Stock { get; private set; }

    public ProductoSimple(string sku, string nombre, Dinero precio, decimal pesoKg, int stock)
    {
        Sku = sku;
        Nombre = nombre;
        Precio = precio;
        PesoKg = pesoKg;
        Stock = stock;
    }

    public void DescontarStock(int cantidad)
    {
        if (cantidad > Stock)
            throw new ExcepcionDominio($"Stock insuficiente para el SKU {Sku}. Disponible: {Stock}, Solicitado: {cantidad}");
        Stock -= cantidad;
    }

    public void ReintegrarStock(int cantidad) => Stock += cantidad;

    public int ObtenerStockMinimo() => Stock;

    public IEnumerable<(string Sku, int Cantidad)> ExpandirComponentes()
    {
        yield return (Sku, 1);
    }
}

public class KitCompuesto : IElementoCatalogo
{
    public string Sku { get; }
    public string Nombre { get; }
    private readonly List<IElementoCatalogo> _componentes = new();
    public IReadOnlyCollection<IElementoCatalogo> Componentes => _componentes.AsReadOnly();

    public Dinero Precio => CalcularPrecioTotal();
    public decimal PesoKg => CalcularPesoTotal();

    public KitCompuesto(string sku, string nombre)
    {
        Sku = sku;
        Nombre = nombre;
    }

    public void AgregarComponente(IElementoCatalogo componente)
    {
        if (componente.Sku == Sku)
            throw new ExcepcionDominio("Un kit no puede contenerse a sí mismo.");
        _componentes.Add(componente);
    }

    private Dinero CalcularPrecioTotal()
    {
        decimal total = 0;
        foreach (var comp in _componentes)
            total += comp.Precio.Valor;
        return new Dinero(total);
    }

    private decimal CalcularPesoTotal()
    {
        decimal peso = 0;
        foreach (var comp in _componentes)
            peso += comp.PesoKg;
        return peso;
    }

    public int ObtenerStockMinimo()
    {
        // Retorna el stock limitante basado en los componentes expandidos
        int min = int.MaxValue;
        foreach (var comp in _componentes)
        {
            int stockComp = comp.ObtenerStockMinimo();
            if (stockComp < min) min = stockComp;
        }
        return min == int.MaxValue ? 0 : min;
    }

    public IEnumerable<(string Sku, int Cantidad)> ExpandirComponentes()
    {
        var acumulados = new Dictionary<string, int>();
        foreach (var comp in _componentes)
        {
            foreach (var sub in comp.ExpandirComponentes())
            {
                if (acumulados.ContainsKey(sub.Sku))
                    acumulados[sub.Sku] += sub.Cantidad;
                else
                    acumulados[sub.Sku] = sub.Cantidad;
            }
        }
        foreach (var par in acumulados)
        {
            yield return (par.Key, par.Value);
        }
    }
}
