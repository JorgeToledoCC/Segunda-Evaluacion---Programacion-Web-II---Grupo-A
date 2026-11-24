using System.Net.Http.Headers;

namespace WebApplication2.Services
{
    /// <summary>
    /// Agrega el JWT del usuario logueado a cada llamada a la API.
    /// </summary>
    public class ApiTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public ApiTokenHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _accessor.HttpContext?.User?.FindFirst("jwt")?.Value;

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
