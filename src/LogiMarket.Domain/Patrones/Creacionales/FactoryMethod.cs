using System;

namespace LogiMarket.Domain.Patrones.Creacionales;

// 1. Producto (Contrato de Notificación)
public interface INotificador
{
    string Tipo { get; }
    void Enviar(string destino, string mensaje);
}

// Productos Concretos
public class NotificadorEmail : INotificador
{
    public string Tipo => "EMAIL";
    public void Enviar(string destino, string mensaje)
    {
        // Simulación de envío por Email
        Console.WriteLine($"[EMAIL ENVIADO a {destino}]: {mensaje}");
    }
}

public class NotificadorSms : INotificador
{
    public string Tipo => "SMS";
    public void Enviar(string destino, string mensaje)
    {
        // Simulación de envío por SMS
        Console.WriteLine($"[SMS ENVIADO a {destino}]: {mensaje}");
    }
}

// 2. Creador Abstracto (Factory Method)
public abstract class CreadorNotificador
{
    public abstract INotificador CrearNotificador();

    public void ProcesarNotificacion(string destino, string mensaje)
    {
        var notificador = CrearNotificador();
        notificador.Enviar(destino, mensaje);
    }
}

// Creadores Concretos
public class CreadorNotificadorEmail : CreadorNotificador
{
    public override INotificador CrearNotificador() => new NotificadorEmail();
}

public class CreadorNotificadorSms : CreadorNotificador
{
    public override INotificador CrearNotificador() => new NotificadorSms();
}
