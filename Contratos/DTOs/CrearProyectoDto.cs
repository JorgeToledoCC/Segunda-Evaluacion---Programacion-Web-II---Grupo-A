using System.ComponentModel.DataAnnotations;

namespace Contratos.DTOs
{
    public class CrearProyectoDto
    {
        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Activo";

        [Required(ErrorMessage = "Debe seleccionar un jefe")]
        public Guid IdJefe { get; set; }
    }
}
