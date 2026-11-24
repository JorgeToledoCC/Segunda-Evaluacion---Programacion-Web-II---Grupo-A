using Contratos.Data;
using WebApiWeb.Models;
using WebApiWeb.Services;

namespace WebApiWeb.Factories
{
    static class UsuarioFactories
    {
        public static void Seed(BaseDatos context)
        {
            if (context.Usuarios.Any())
            {
                return;
            }

            context.Usuarios.Add(new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = "admin",
                Rol = "Admin",
                PasswordHash = Seguridad.Hashear("admin123")
            });

            context.SaveChanges();
        }
    }
}
