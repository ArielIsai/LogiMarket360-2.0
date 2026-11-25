using System.Collections.Generic;
using System.Threading.Tasks;
using LogiMarket.Domain.Clientes;

namespace LogiMarket.Application.Puertos
{
    public interface IRepositorioClientes
    {
        Task<Cliente> ObtenerPorIdAsync(string identificacion);
        Task<IEnumerable<Cliente>> ObtenerTodosAsync();
        Task AgregarAsync(Cliente cliente);
        Task ActualizarAsync(Cliente cliente);
    }
}
