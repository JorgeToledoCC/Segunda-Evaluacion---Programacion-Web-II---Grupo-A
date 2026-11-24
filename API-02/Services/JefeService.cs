using Contratos.Data;
using Contratos.DTOs;
using Contratos.Interfaces;
using Contratos.Models;

namespace Contratos.Services
{
    public class JefeService : IJefeInterface
    {
        private readonly BaseDatos _context;

        public JefeService(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Jefe> ObtenerTodos()
        {
            return _context.Jefes.OrderBy(j => j.Nombre).ToList();
        }

        public Jefe? ObtenerPorId(Guid id)
        {
            return _context.Jefes.Find(id);
        }

        public Jefe Crear(CrearJefeDto dto)
        {
            var nombre = (dto.Nombre ?? string.Empty).Trim();

            if (_context.Jefes.Any(j => j.Nombre.ToLower() == nombre.ToLower()))
            {
                throw new InvalidOperationException("Ya existe un jefe con ese nombre.");
            }

            var jefe = new Jefe
            {
                Id = GenerarIdUnico(),
                Nombre = nombre,
                Cargo = dto.Cargo,
                Correo = dto.Correo
            };

            _context.Jefes.Add(jefe);
            _context.SaveChanges();

            return jefe;
        }

        private Guid GenerarIdUnico()
        {
            Guid id;
            do
            {
                id = Guid.NewGuid();
            }
            while (_context.Jefes.Any(j => j.Id == id));

            return id;
        }
    }
}
