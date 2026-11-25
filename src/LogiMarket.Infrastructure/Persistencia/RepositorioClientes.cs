using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using LogiMarket.Domain.Clientes;
using LogiMarket.Application.Puertos;

namespace LogiMarket.Infrastructure.Persistencia
{
    public class RepositorioClientes : IRepositorioClientes
    {
        private readonly LogiMarketDbContext _context;

        public RepositorioClientes(LogiMarketDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente> ObtenerPorIdAsync(string identificacion)
        {
            return await _context.Clientes.FindAsync(identificacion);
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task AgregarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }
    }
}
