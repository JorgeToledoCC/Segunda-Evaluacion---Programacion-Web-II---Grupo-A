namespace Contratos.DTOs
{
    public class ProyectoDto
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string Estado { get; set; } = string.Empty;

        public Guid IdJefe { get; set; }

        // Datos del jefe (para el listado y la busqueda por id)
        public string? JefeNombre { get; set; }

        public string? JefeCargo { get; set; }

        public string? JefeCorreo { get; set; }
    }
}
