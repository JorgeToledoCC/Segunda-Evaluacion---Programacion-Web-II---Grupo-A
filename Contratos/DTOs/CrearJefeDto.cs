using System.ComponentModel.DataAnnotations;

namespace Contratos.DTOs
{
    public class CrearJefeDto
    {
        [Required(ErrorMessage = "El nombre del jefe es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Cargo { get; set; }

        [EmailAddress(ErrorMessage = "El correo no es válido")]
        [StringLength(150)]
        public string? Correo { get; set; }
    }
}
