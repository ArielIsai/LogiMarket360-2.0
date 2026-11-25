using System;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Clientes;

public enum CategoriaCliente
{
    Normal = 0,
    Vip = 1,
    Mayorista = 2
}

public class Cliente
{
    public string Identificacion { get; private set; } = string.Empty;
    public string Nombre { get; private set; } = string.Empty;
    public CategoriaCliente Categoria { get; private set; }
    public bool Activo { get; private set; }

    // Constructor protegido para EF Core
    protected Cliente() { }

    public Cliente(string identificacion, string nombre, CategoriaCliente categoria)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
            throw new ExcepcionDominio("La identificación del cliente es obligatoria.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionDominio("El nombre o razón social del cliente es obligatorio.");

        Identificacion = identificacion.Trim();
        Nombre = nombre.Trim();
        Categoria = categoria;
        Activo = true;
    }

    public void ActualizarDatos(string nombre, CategoriaCliente categoria)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionDominio("El nombre no puede estar vacío.");
        
        Nombre = nombre.Trim();
        Categoria = categoria;
    }

    public void Desactivar() => Activo = false;
}
