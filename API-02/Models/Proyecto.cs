using System.ComponentModel.DataAnnotations;

namespace Contratos.Models
{
    public class Proyecto
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string Estado { get; set; } = "Activo";

        public Guid IdJefe { get; set; }

        public Jefe? Jefe { get; set; }
    }
}
