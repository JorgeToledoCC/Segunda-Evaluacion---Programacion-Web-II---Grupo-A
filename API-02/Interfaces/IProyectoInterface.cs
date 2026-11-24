using Contratos.DTOs;

namespace Contratos.Interfaces
{
    public interface IProyectoInterface
    {
        IEnumerable<ProyectoDto> ObtenerTodos();
        ProyectoDto? ObtenerPorId(Guid id);
        ProyectoDto Crear(CrearProyectoDto dto);
    }
}
