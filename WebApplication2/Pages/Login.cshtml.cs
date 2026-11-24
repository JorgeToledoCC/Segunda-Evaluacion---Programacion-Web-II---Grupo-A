using System.Net.Http.Json;
using System.Security.Claims;
using Contratos.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication2.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly HttpClient _http;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("Api");
        }

        [BindProperty]
        public LoginDto Login { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public string? MensajeError { get; private set; }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Redirigir(ReturnUrl);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/Auth/login", Login);

                if (respuesta.IsSuccessStatusCode)
                {
                    var datos = await respuesta.Content.ReadFromJsonAsync<LoginRespuestaDTO>();

                    if (datos is not null)
                    {
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, datos.NombreUsuario),
                            new(ClaimTypes.Role, datos.Rol),
                            new("jwt", datos.Token)
                        };

                        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identidad));

                        TempData["Mensaje"] = $"Bienvenido, {datos.NombreUsuario}";
                        return Redirigir(ReturnUrl);
                    }
                }

                MensajeError = "Usuario o contraseña incorrectos.";
            }
            catch (Exception ex)
            {
                MensajeError = "No se pudo conectar con la API: " + ex.Message;
            }

            return Page();
        }

        private IActionResult Redirigir(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage("/Index");
        }
    }
}
