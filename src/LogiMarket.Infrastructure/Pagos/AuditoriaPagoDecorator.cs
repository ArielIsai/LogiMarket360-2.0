using System;
using System.Threading.Tasks;
using LogiMarket.Application.Puertos;

namespace LogiMarket.Infrastructure.Pagos;

public class AuditoriaPagoDecorator : IPasarelaPago
{
    private readonly IPasarelaPago _pasarelaInterna;

    public AuditoriaPagoDecorator(IPasarelaPago pasarelaInterna)
    {
        _pasarelaInterna = pasarelaInterna;
    }

    public async Task<(bool Exitoso, string Referencia)> CobrarAsync(decimal monto, string claveIdempotencia)
    {
        string correlacionId = Guid.NewGuid().ToString()[..8];
        Console.WriteLine($"[LOG - {correlacionId}] INICIO de cobro por USD {monto:N2} (Clave: {claveIdempotencia})");

        try
        {
            var resultado = await _pasarelaInterna.CobrarAsync(monto, claveIdempotencia);
            if (resultado.Exitoso)
            {
                Console.WriteLine($"[LOG - {correlacionId}] FIN: Cobro APROBADO. Ref: {resultado.Referencia}");
            }
            else
            {
                Console.WriteLine($"[LOG - {correlacionId}] FIN: Cobro RECHAZADO por pasarela. Ref: {resultado.Referencia}");
            }
            return resultado;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LOG - {correlacionId}] ERROR crítico en pasarela: {ex.Message}");
            throw;
        }
    }
}
