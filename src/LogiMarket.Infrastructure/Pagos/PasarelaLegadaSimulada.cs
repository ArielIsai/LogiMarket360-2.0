namespace LogiMarket.Infrastructure.Pagos;

public class PasarelaLegadaSimulada
{
    public string CobrarCentavos(long centavos, string tokenSeguridad)
    {
        // Simulación de pasarela legada con interfaz incompatible
        if (centavos <= 0) return "RECHAZADO_MONTO";
        return $"LEGACY-OK-{tokenSeguridad[..Math.Min(tokenSeguridad.Length, 6)]}";
    }
}
