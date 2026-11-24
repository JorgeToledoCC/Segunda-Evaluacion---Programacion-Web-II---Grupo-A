using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.Jefes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        public List<JefeDto> Jefes { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                Jefes = await _http.GetFromJsonAsync<List<JefeDto>>("api/Jefe") ?? new();
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudieron cargar los jefes: " + ex.Message;
            }
        }
    }
}
