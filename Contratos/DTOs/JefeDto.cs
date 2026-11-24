using System.ComponentModel.DataAnnotations;

namespace Contratos.DTOs
{
    public class JefeDto
    {
        public Guid Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Cargo { get; set; }

        public string? Correo { get; set; }
    }
}
