using System.Threading.Tasks;

namespace LogiMarket.Application.Puertos;

public interface IPasarelaPago
{
    Task<(bool Exitoso, string Referencia)> CobrarAsync(decimal monto, string claveIdempotencia);
}
