using System;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Pagos;

public class Pago
{
    public Guid Id { get; private set; }
    public Guid PedidoId { get; private set; }
    public Dinero Monto { get; private set; }
    public string ReferenciaTransaccion { get; private set; } = string.Empty;
    public string ClaveIdempotencia { get; private set; } = string.Empty;
    public DateTime FechaPago { get; private set; }
    public bool Exitoso { get; private set; }

    protected Pago() { }

    public Pago(Guid pedidoId, Dinero monto, string referenciaTransaccion, string claveIdempotencia, bool exitoso)
    {
        if (monto.Valor <= 0)
            throw new ExcepcionDominio("El monto del pago debe ser mayor a cero.");
        if (string.IsNullOrWhiteSpace(referenciaTransaccion))
            throw new ExcepcionDominio("La referencia de transacción es obligatoria.");
        if (string.IsNullOrWhiteSpace(claveIdempotencia))
            throw new ExcepcionDominio("La clave de idempotencia es obligatoria para prevenir cobros duplicados.");

        Id = Guid.NewGuid();
        PedidoId = pedidoId;
        Monto = monto;
        ReferenciaTransaccion = referenciaTransaccion.Trim();
        ClaveIdempotencia = claveIdempotencia.Trim();
        FechaPago = DateTime.UtcNow;
        Exitoso = exitoso;
    }
}
