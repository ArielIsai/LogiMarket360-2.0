using System;

namespace LogiMarket.Domain.Comun;

public readonly struct Dinero : IEquatable<Dinero>
{
    public decimal Valor { get; }

    public Dinero(decimal valor)
    {
        if (valor < 0)
            throw new ExcepcionDominio("El monto monetario no puede ser negativo.");
        
        Valor = decimal.Round(valor, 2, MidpointRounding.AwayFromZero);
    }

    public static Dinero Zero => new(0.00m);

    public static Dinero operator +(Dinero a, Dinero b) => new(a.Valor + b.Valor);
    public static Dinero operator -(Dinero a, Dinero b) => new(a.Valor - b.Valor);
    public static Dinero operator *(Dinero a, decimal multiplicador) => new(a.Valor * multiplicador);

    public bool Equals(Dinero other) => Valor == other.Valor;
    public override bool Equals(object? obj) => obj is Dinero other && Equals(other);
    public override int GetHashCode() => Valor.GetHashCode();
    public override string ToString() => $"USD {Valor:N2}";
}
