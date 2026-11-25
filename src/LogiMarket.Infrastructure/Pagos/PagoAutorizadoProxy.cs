using System;
using System.Threading.Tasks;
using LogiMarket.Application.Puertos;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Infrastructure.Pagos;

public class PagoAutorizadoProxy : IPasarelaPago
{
    private readonly IPasarelaPago _pasarelaReal;
    private readonly string _rolUsuarioActual;

    public PagoAutorizadoProxy(IPasarelaPago pasarelaReal, string rolUsuarioActual)
    {
        _pasarelaReal = pasarelaReal;
        _rolUsuarioActual = rolUsuarioActual?.ToUpper() ?? "CONSULTA";
    }

    public async Task<(bool Exitoso, string Referencia)> CobrarAsync(decimal monto, string claveIdempotencia)
    {
        // Control estricto de seguridad: Solo roles VENTAS o ADMIN pueden ejecutar cobros
        if (_rolUsuarioActual != "VENTAS" && _rolUsuarioActual != "ADMIN")
        {
            Console.WriteLine($"[SEGURIDAD PROXY] Acceso DENEGADO para rol '{_rolUsuarioActual}' al intentar procesar cobro.");
            throw new ExcepcionDominio($"Acceso denegado: El rol '{_rolUsuarioActual}' no tiene permisos para ejecutar cobros de pasarela.");
        }

        // Si está autorizado, delega en la pasarela real (que a su vez está envuelta por el Decorator)
        return await _pasarelaReal.CobrarAsync(monto, claveIdempotencia);
    }
}
