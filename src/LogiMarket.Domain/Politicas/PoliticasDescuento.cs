using LogiMarket.Domain.Clientes;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Politicas;

public interface IPoliticaDescuento
{
    string Nombre { get; }
    decimal CalcularTasa(CategoriaCliente categoriaCliente, int unidadesFisicasTotales);
}

public class PoliticaDescuentoMayorista : IPoliticaDescuento
{
    public string Nombre => "MAYORISTA";
    public decimal CalcularTasa(CategoriaCliente categoriaCliente, int unidadesFisicasTotales)
    {
        // Mayorista aplica 15% si el total de unidades físicas finales es al menos 10, sin importar categoría
        return unidadesFisicasTotales >= 10 ? 0.15m : 0.00m;
    }
}

public class PoliticaDescuentoVip : IPoliticaDescuento
{
    public string Nombre => "VIP";
    public decimal CalcularTasa(CategoriaCliente categoriaCliente, int unidadesFisicasTotales)
    {
        return categoriaCliente == CategoriaCliente.Vip ? 0.10m : 0.00m;
    }
}

public class PoliticaDescuentoNormal : IPoliticaDescuento
{
    public string Nombre => "NORMAL";
    public decimal CalcularTasa(CategoriaCliente categoriaCliente, int unidadesFisicasTotales)
    {
        return 0.00m;
    }
}

public class CalculadorDescuentoContexto
{
    private readonly IPoliticaDescuento[] _politicas = new IPoliticaDescuento[]
    {
        new PoliticaDescuentoMayorista(),
        new PoliticaDescuentoVip(),
        new PoliticaDescuentoNormal()
    };

    public (string PoliticaAplicada, decimal Tasa) ObtenerMejorDescuento(CategoriaCliente categoria, int unidadesFisicas)
    {
        // Prioridad: Mayorista tiene precedencia si cumple la regla de unidades, luego VIP, luego Normal
        foreach (var politica in _politicas)
        {
            decimal tasa = politica.CalcularTasa(categoria, unidadesFisicas);
            if (tasa > 0)
            {
                return (politica.Nombre, tasa);
            }
        }
        return ("NORMAL", 0.00m);
    }
}
