using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContratoProyectos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendProyectos.Pages.Jefes
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public JefeDTOs NuevoJefe { get; set; } = new();

        public List<JefeItem> ListaJefes { get; set; } = new();

        [TempData]
        public string? Mensaje { get; set; }

        [TempData]
        public bool EsError { get; set; }

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        private const int RegistrosPorPagina = 5;

        private HttpClient? CrearClienteAutenticado()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return null;

            var client = _httpClientFactory.CreateClient("Api");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<IActionResult> OnGetAsync(int pagina = 1)
        {
            var client = CrearClienteAutenticado();
            if (client == null) return RedirectToPage("/Auth/Login");

            var autorizado = await CargarJefes(client, pagina);
            if (!autorizado)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = CrearClienteAutenticado();
            if (client == null) return RedirectToPage("/Auth/Login");

            if (!ModelState.IsValid)
            {
                await CargarJefes(client, 1);
                return Page();
            }

            var respuesta = await client.PostAsJsonAsync("api/Jefe", NuevoJefe);
            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Auth/Login");
            }

            var textoRespuesta = await respuesta.Content.ReadAsStringAsync();
            EsError = !respuesta.IsSuccessStatusCode;
            Mensaje = string.IsNullOrWhiteSpace(textoRespuesta)
                ? (respuesta.IsSuccessStatusCode ? "Jefe registrado correctamente." : "Error al registrar Jefe.")
                : textoRespuesta.Trim('"');

            return RedirectToPage();
        }

        private async Task<bool> CargarJefes(HttpClient client, int pagina)
        {
            var response = await client.GetAsync("api/Jefe");
            if (response.StatusCode == HttpStatusCode.Unauthorized) return false;

            response.EnsureSuccessStatusCode();
            var resultado = await response.Content.ReadFromJsonAsync<List<JefeItem>>();

            if (resultado != null)
            {
                TotalRegistros = resultado.Count;
                TotalPaginas = Math.Max(1, (int)Math.Ceiling(TotalRegistros / (double)RegistrosPorPagina));
                PaginaActual = Math.Clamp(pagina, 1, TotalPaginas);

                ListaJefes = resultado
                    .Skip((PaginaActual - 1) * RegistrosPorPagina)
                    .Take(RegistrosPorPagina)
                    .ToList();
            }

            return true;
        }

        public class JefeItem
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Cargo { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
        }
    }
}
