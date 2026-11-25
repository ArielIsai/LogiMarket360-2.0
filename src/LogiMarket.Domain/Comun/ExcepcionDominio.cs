using System;

namespace LogiMarket.Domain.Comun;

public class ExcepcionDominio : Exception
{
    public ExcepcionDominio(string mensaje) : base(mensaje) { }
}
