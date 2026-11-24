using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.Proyectos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly HttpClient _http;

        public IndexModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        public List<ProyectoDto> Proyectos { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                Proyectos = await _http.GetFromJsonAsync<List<ProyectoDto>>("api/Proyecto") ?? new();
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudieron cargar los proyectos: " + ex.Message;
            }
        }
    }
}
