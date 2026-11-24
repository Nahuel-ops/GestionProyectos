using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ContratoProyectos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontendProyectos.Pages.Proyectos
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public ProyectoDTOs NuevoProyecto { get; set; } = new();

        public List<ProyectoItem> ListaProyectos { get; set; } = new();
        public List<JefeOpcion> JefesDisponibles { get; set; } = new();
        public List<JefeOpcion> TodosLosJefes { get; set; } = new();

        public ProyectoItem? ProyectoEncontrado { get; set; }
        public string? IdBuscado { get; set; }
        public string? ErrorBusqueda { get; set; }

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

        public async Task<IActionResult> OnGetAsync(int pagina = 1, string? buscarId = null)
        {
            var client = CrearClienteAutenticado();
            if (client == null) return RedirectToPage("/Auth/Login");

            var autorizado = await CargarDatos(client, pagina);
            if (!autorizado)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Auth/Login");
            }

            if (!string.IsNullOrWhiteSpace(buscarId))
            {
                IdBuscado = buscarId.Trim();
                if (Guid.TryParse(IdBuscado, out Guid guidProyecto))
                {
                    var respDetalle = await client.GetAsync($"api/Proyecto/{guidProyecto}");
                    if (respDetalle.IsSuccessStatusCode)
                    {
                        ProyectoEncontrado = await respDetalle.Content.ReadFromJsonAsync<ProyectoItem>();
                    }
                    else
                    {
                        ErrorBusqueda = "No se encontró ningún proyecto con ese ID.";
                    }
                }
                else
                {
                    ErrorBusqueda = "El formato del ID (Guid) ingresado no es válido.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = CrearClienteAutenticado();
            if (client == null) return RedirectToPage("/Auth/Login");

            if (!ModelState.IsValid)
            {
                await CargarDatos(client, 1);
                return Page();
            }

            var respuesta = await client.PostAsJsonAsync("api/Proyecto", NuevoProyecto);
            if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Auth/Login");
            }

            var textoRespuesta = await respuesta.Content.ReadAsStringAsync();
            EsError = !respuesta.IsSuccessStatusCode;
            Mensaje = string.IsNullOrWhiteSpace(textoRespuesta)
                ? (respuesta.IsSuccessStatusCode ? "Proyecto registrado correctamente." : "Error al registrar proyecto.")
                : textoRespuesta.Trim('"');

            return RedirectToPage();
        }

        private async Task<bool> CargarDatos(HttpClient client, int pagina)
        {
            var respProyectos = await client.GetAsync("api/Proyecto");
            if (respProyectos.StatusCode == HttpStatusCode.Unauthorized) return false;
            respProyectos.EnsureSuccessStatusCode();

            var proyectos = await respProyectos.Content.ReadFromJsonAsync<List<ProyectoItem>>();
            if (proyectos != null)
            {
                TotalRegistros = proyectos.Count;
                TotalPaginas = Math.Max(1, (int)Math.Ceiling(TotalRegistros / (double)RegistrosPorPagina));
                PaginaActual = Math.Clamp(pagina, 1, TotalPaginas);

                ListaProyectos = proyectos
                    .Skip((PaginaActual - 1) * RegistrosPorPagina)
                    .Take(RegistrosPorPagina)
                    .ToList();
            }

            JefesDisponibles = await client.GetFromJsonAsync<List<JefeOpcion>>("api/Jefe/disponibles") ?? new();
            TodosLosJefes = await client.GetFromJsonAsync<List<JefeOpcion>>("api/Jefe") ?? new();

            return true;
        }

        public class ProyectoItem
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public string Estado { get; set; } = string.Empty;
            public Guid IdJefe { get; set; }
            public JefeOpcion? Jefe { get; set; }
        }

        public class JefeOpcion
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Cargo { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
        }
    }
}
