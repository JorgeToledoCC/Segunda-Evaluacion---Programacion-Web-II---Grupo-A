using System.Net.Http.Json;
using System.Text.Json;
using Contratos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages.Jefes
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
        public CrearJefeDto Jefe { get; set; } = new();

        public string? MensajeError { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/Jefe", Jefe);

                if (!respuesta.IsSuccessStatusCode)
                {
                    var json = await respuesta.Content.ReadAsStringAsync();
                    MensajeError = LeerMensaje(json) ?? "No se pudo crear el jefe.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo conectar con la API: " + ex.Message;
                return Page();
            }

            TempData["Mensaje"] = "Jefe registrado correctamente";
            return RedirectToPage("Index");
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
