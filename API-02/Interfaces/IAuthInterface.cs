using Contratos.DTOs;

namespace WebApiWeb.Interfaces
{
    public interface IAuthInterface
    {
        void Registrar(LoginDto dto);
        LoginRespuestaDTO? Login(LoginDto dto);
    }
}
