using Contratos.DTOs;
using Contratos.Models;

namespace Contratos.Interfaces
{
    public interface IJefeInterface
    {
        IEnumerable<Jefe> ObtenerTodos();
        Jefe? ObtenerPorId(Guid id);
        Jefe Crear(CrearJefeDto dto);
    }
}
