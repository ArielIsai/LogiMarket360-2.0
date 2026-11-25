using System.Threading.Tasks;
using LogiMarket.Application.Puertos;

namespace LogiMarket.Infrastructure.Pagos;

public class AdaptadorPasarelaLegada : IPasarelaPago
{
    private readonly PasarelaLegadaSimulada _pasarelaLegada;

    public AdaptadorPasarelaLegada(PasarelaLegadaSimulada pasarelaLegada)
    {
        _pasarelaLegada = pasarelaLegada;
    }

    public async Task<(bool Exitoso, string Referencia)> CobrarAsync(decimal monto, string claveIdempotencia)
    {
        // Convierte decimal USD a centavos (long) exigido por el sistema legado
        long centavos = (long)(monto * 100);
        
        // Ejecuta la operación incompatible adaptándola al contrato moderno
        string resultado = _pasarelaLegada.CobrarCentavos(centavos, claveIdempotencia);
        
        bool exitoso = resultado.StartsWith("LEGACY-OK");
        return await Task.FromResult((exitoso, resultado));
    }
}
