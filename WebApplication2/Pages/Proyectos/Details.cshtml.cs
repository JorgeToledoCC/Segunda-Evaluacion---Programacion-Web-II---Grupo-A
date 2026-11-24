using System.Net;
using System.Net.Http.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.Proyectos
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _http;

        public DetailsModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        [FromQuery]
        public Guid Id { get; set; }

        public ProyectoDto? Proyecto { get; private set; }

        public string? MensajeError { get; private set; }

        public async Task OnGetAsync()
        {
            if (Id == Guid.Empty)
            {
                MensajeError = "Ingresá un ID de proyecto para buscar.";
                return;
            }

            try
            {
                var respuesta = await _http.GetAsync($"api/Proyecto/{Id}");

                if (respuesta.IsSuccessStatusCode)
                {
                    Proyecto = await respuesta.Content.ReadFromJsonAsync<ProyectoDto>();
                }
                else if (respuesta.StatusCode == HttpStatusCode.NotFound)
                {
                    MensajeError = "No se encontró un proyecto con ese ID.";
                }
                else
                {
                    MensajeError = $"Error {(int)respuesta.StatusCode} al buscar el proyecto.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo conectar con la API: " + ex.Message;
            }
        }
    }
}
