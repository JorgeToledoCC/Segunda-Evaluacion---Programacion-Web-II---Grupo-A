using System.ComponentModel.DataAnnotations;

namespace Contratos.Models
{
    public class Jefe
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del jefe es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string? Cargo { get; set; }

        public string? Correo { get; set; }
    }
}
