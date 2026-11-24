using Contratos.Data;
using Contratos.DTOs;
using Contratos.Interfaces;
using Contratos.Models;
using Microsoft.EntityFrameworkCore;

namespace Contratos.Services
{
    public class ProyectoService : IProyectoInterface
    {
        private readonly BaseDatos _context;

        public ProyectoService(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<ProyectoDto> ObtenerTodos()
        {
            return _context.Proyectos
                .Include(p => p.Jefe)
                .OrderBy(p => p.Nombre)
                .Select(p => new ProyectoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado,
                    IdJefe = p.IdJefe,
                    JefeNombre = p.Jefe != null ? p.Jefe.Nombre : null,
                    JefeCargo = p.Jefe != null ? p.Jefe.Cargo : null,
                    JefeCorreo = p.Jefe != null ? p.Jefe.Correo : null
                })
                .ToList();
        }

        public ProyectoDto? ObtenerPorId(Guid id)
        {
            return _context.Proyectos
                .Include(p => p.Jefe)
                .Where(p => p.Id == id)
                .Select(p => new ProyectoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado,
                    IdJefe = p.IdJefe,
                    JefeNombre = p.Jefe != null ? p.Jefe.Nombre : null,
                    JefeCargo = p.Jefe != null ? p.Jefe.Cargo : null,
                    JefeCorreo = p.Jefe != null ? p.Jefe.Correo : null
                })
                .FirstOrDefault();
        }

        public ProyectoDto Crear(CrearProyectoDto dto)
        {
            var nombre = (dto.Nombre ?? string.Empty).Trim();

            if (_context.Proyectos.Any(p => p.Nombre.ToLower() == nombre.ToLower()))
            {
                throw new InvalidOperationException("Ya existe un proyecto con ese nombre.");
            }

            if (!_context.Jefes.Any(j => j.Id == dto.IdJefe))
            {
                throw new ArgumentException("El jefe seleccionado no existe.");
            }

            if (_context.Proyectos.Any(p => p.IdJefe == dto.IdJefe))
            {
                throw new InvalidOperationException("Ese jefe ya está a cargo de otro proyecto.");
            }

            var proyecto = new Proyecto
            {
                Id = GenerarIdUnico(),
                Nombre = nombre,
                Descripcion = dto.Descripcion,
                Estado = string.IsNullOrWhiteSpace(dto.Estado) ? "Activo" : dto.Estado.Trim(),
                IdJefe = dto.IdJefe
            };

            _context.Proyectos.Add(proyecto);
            _context.SaveChanges();

            return ObtenerPorId(proyecto.Id)!;
        }

        private Guid GenerarIdUnico()
        {
            Guid id;
            do
            {
                id = Guid.NewGuid();
            }
            while (_context.Proyectos.Any(p => p.Id == id));

            return id;
        }
    }
}
