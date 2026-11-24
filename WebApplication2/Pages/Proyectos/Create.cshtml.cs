using System.Net.Http.Json;
using System.Text.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication2.Pages.Proyectos
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly HttpClient _http;

        public CreateModel(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("Api");
        }

        [BindProperty]
        public CrearProyectoDto Proyecto { get; set; } = new();

        public List<SelectListItem> Jefes { get; private set; } = new();
        public string? MensajeError { get; private set; }

        public async Task OnGetAsync()
        {
            await CargarJefesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Proyecto.IdJefe == Guid.Empty)
            {
                ModelState.AddModelError("Proyecto.IdJefe", "Debe seleccionar un jefe.");
            }

            if (!ModelState.IsValid)
            {
                await CargarJefesAsync();
                return Page();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/Proyecto", Proyecto);

                if (!respuesta.IsSuccessStatusCode)
                {
                    var json = await respuesta.Content.ReadAsStringAsync();
                    MensajeError = LeerMensaje(json) ?? "No se pudo crear el proyecto.";
                    await CargarJefesAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo conectar con la API: " + ex.Message;
                await CargarJefesAsync();
                return Page();
            }

            TempData["Mensaje"] = "Proyecto registrado correctamente";
            return RedirectToPage("Index");
        }

        private async Task CargarJefesAsync()
        {
            try
            {
                var jefes = await _http.GetFromJsonAsync<List<JefeDto>>("api/Jefe") ?? new();
                Jefes = jefes.Select(j => new SelectListItem(
                    $"{j.Nombre} — {j.Cargo}", j.Id.ToString())).ToList();
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudieron cargar los jefes: " + ex.Message;
            }
        }

        private static string? LeerMensaje(string json)
        {
            try
            {
                using var documento = JsonDocument.Parse(json);
                return documento.RootElement.TryGetProperty("mensaje", out var mensaje)
                    ? mensaje.GetString()
                    : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
