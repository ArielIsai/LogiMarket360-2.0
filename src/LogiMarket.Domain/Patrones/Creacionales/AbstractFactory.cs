using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Patrones.Creacionales;

public interface ICalculadorTarifa { Dinero ObtenerTarifa(); }
public interface ICalculadorPlazo { int ObtenerDiasPlazo(); }
public interface IConstructorGuia { string GenerarGuia(string idPedido); }

public interface IFabricaEntrega
{
    ICalculadorTarifa CrearTarifa();
    ICalculadorPlazo CrearPlazo();
    IConstructorGuia CrearGuia();
}

public class TarifaEstandar : ICalculadorTarifa { public Dinero ObtenerTarifa() => new Dinero(5.00m); }
public class PlazoEstandar : ICalculadorPlazo { public int ObtenerDiasPlazo() => 3; }
public class GuiaEstandar : IConstructorGuia { public string GenerarGuia(string idPedido) => $"GUIA-EST-{idPedido}"; }

public class FabricaEntregaEstandar : IFabricaEntrega
{
    public ICalculadorTarifa CrearTarifa() => new TarifaEstandar();
    public ICalculadorPlazo CrearPlazo() => new PlazoEstandar();
    public IConstructorGuia CrearGuia() => new GuiaEstandar();
}

public class TarifaExpres : ICalculadorTarifa { public Dinero ObtenerTarifa() => new Dinero(12.00m); }
public class PlazoExpres : ICalculadorPlazo { public int ObtenerDiasPlazo() => 1; }
public class GuiaExpres : IConstructorGuia { public string GenerarGuia(string idPedido) => $"GUIA-EXP-{idPedido}"; }

public class FabricaEntregaExpres : IFabricaEntrega
{
    public ICalculadorTarifa CrearTarifa() => new TarifaExpres();
    public ICalculadorPlazo CrearPlazo() => new PlazoExpres();
    public IConstructorGuia CrearGuia() => new GuiaExpres();
}
